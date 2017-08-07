using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;

namespace WaitingRoomBigScreen.WebService
{
    public class DataHandler
    {
        protected long _count;

        static DataHandler()
        {
            // Windows XP
            var os = Environment.OSVersion;
            if (os.Platform != PlatformID.Win32NT || os.Version.Major < 6)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            else
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(
                    (int)SecurityProtocolType.Tls
                    | (int)SecurityProtocolType.Ssl3
                    | 768 /* SecurityProtocolType.Tls11 */
                    | 3072 /* SecurityProtocolType.Tls12 */);
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
        }

        protected static ILog Logger => LogManager.GetLogger("Network");

        public Uri Uri { get; set; }

        public static string token = string.Empty;

        public bool Local { get; set; }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
        {
            return true; //总是接受
        }
        public  async Task<Result<TRes>> Query<TRes, TReq>(TReq req) where TRes : IRes, new()
            where TReq : IReq
        {
            var i = Interlocked.Increment(ref _count);
            Logger.Info($"[{i}] [{req.serviceUrl}] Send: {JsonConvert.SerializeObject(req)}");
            try
            {
                var watch = Stopwatch.StartNew();
                var formContent = new FormUrlEncodedContent(req.GetParams());
                Uri fullUri;
                Uri.TryCreate(Uri, req.serviceUrl, out fullUri);
                var response = new HttpClient().Post(fullUri.AbsoluteUri, formContent);
                var text = await response.Content.ReadAsStringAsync();
                watch.Stop();
                var time = watch.ElapsedMilliseconds;
                Logger.Info($"[{i}] [{req.serviceUrl}] Elapsed:{time}ms Recv: {text}");
                if (!response.IsSuccessStatusCode)
                    return Result<TRes>.Fail($"服务器返回状态:{(int)response.StatusCode} {response.StatusCode}");
                var res = JsonConvert.DeserializeObject<TRes>(text);
                return Result<TRes>.Success(res);
            }
            catch (AggregateException ex)
            {
                string mainException;
                Logger.Warn($"[{i}] [{req.serviceUrl}] Exception: {PrintAggregateException(ex, out mainException)}");
                return Result<TRes>.Fail(mainException, ex);
            }
            catch (Exception ex)
            {
                Logger.Warn($"[{i}] [{req.serviceUrl}] Exception: {ex.Message}\n{ex.StackTrace}");
                return Result<TRes>.Fail(ex.Message, ex);
            }
        }

        public string PrintAggregateException(AggregateException ex, out string mainException)
        {
            mainException = string.Empty;
            var sb = new StringBuilder();
            foreach (var inner in ex.InnerExceptions)
            {
                mainException = inner.Message;
                sb.AppendLine($"{inner.Message}\n{inner.StackTrace}");
                var n = 1;
                var pointer = inner.InnerException;
                while (pointer != null)
                {
                    mainException = pointer.Message;
                    sb.AppendLine($"{"".PadLeft(n, '\t')}{pointer.Message}\n{"".PadLeft(n, '\t')}{pointer.StackTrace}");
                    pointer = pointer.InnerException;
                    n++;
                }
            }
            return sb.ToString();
        }
    }
}
