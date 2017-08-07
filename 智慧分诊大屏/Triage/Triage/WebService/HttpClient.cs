using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;

namespace WaitingRoomBigScreen.WebService
{
    public class HttpClient
    {
        public HttpResponse Get(string url)
        {
            var req = new HttpRequest()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            return Do(req);
        }

        public HttpResponse Post(string url, HttpContent content)
        {
            var req = new HttpRequest()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = content,
            };
            return Do(req);
        }

        public HttpResponse Do(HttpRequest req)
        {
            using (var tcpClient = new TcpClient(req.RequestUri.Host, req.RequestUri.Port))
            {
                if (req.RequestUri.Scheme == Uri.UriSchemeHttps)
                {
                    var protocol = new TlsClientProtocol(tcpClient.GetStream(), new SecureRandom());
                    var tlsClient = new MyTlsClient();
                    protocol.Connect(tlsClient);
                    var stream = protocol.Stream;
                    WriteRequest(req, stream);
                    var res = ReadResponse(stream);
                    protocol.Close();
                    return res;
                }
                else
                {
                    var stream = tcpClient.GetStream();
                    WriteRequest(req, stream);
                    var res = ReadResponse(stream);
                    return res;
                }
            }
        }

        private void WriteRequest(HttpRequest req, Stream stream)
        {
            byte[] bytes = null;

            if (req.Content != null)
            {
                foreach (var header in req.Content.Headers)
                {
                    if (req.Headers.ContainsKey(header.Key))
                        req.Headers[header.Key].AddRange(header.Value);
                    else
                        req.Headers[header.Key] = header.Value.ToList();
                }
                bytes = req.Content.ReadAsByteArrayAsync().Result;
                req.Headers["Content-Length"] = new List<string>() { bytes.Length.ToString() };
            }

            var sb = new StringBuilder();
            sb.Append($"{req.Method.Method} /{req.RequestUri.PathAndQuery} HTTP/1.1\r\n");
            sb.Append($"Host: {req.RequestUri.Host}\r\n");
            foreach (var header in req.Headers)
                sb.Append($"{header.Key}: {string.Join(",", header.Value)}\r\n");
            sb.Append("\r\n");

            var request = sb.ToString();
            stream.Write(Encoding.UTF8.GetBytes(request), 0, request.Length);

            if (bytes != null)
                stream.Write(bytes, 0, bytes.Length);

            stream.Flush();
        }

        private HttpResponse ReadResponse(Stream stream)
        {
            var status = ReadLine(stream);

            var p = status.IndexOf(' ');
            //var version = status.Substring(0, p);
            var q = status.IndexOf(' ', p + 1);
            var statusCode = status.Substring(p + 1, q - p);
            var res = new HttpResponse((HttpStatusCode)int.Parse(statusCode));
            var headers = res.Headers;
            while (true)
            {
                var line = ReadLine(stream);

                if (string.IsNullOrEmpty(line))
                    break;
                var i = line.IndexOf(": ", StringComparison.Ordinal);
                var name = line.Substring(0, i);

                if (headers.ContainsKey(name))
                    headers[name].Add(line.Substring(i + 2));
                else
                    headers[name] = new List<string> { line.Substring(i + 2) };
            }
            if (headers.ContainsKey("Content-Length"))
            {
                var length = int.Parse(headers["Content-Length"].First());

                var bytes = new byte[length];
                var n = 0;
                while (n < length)
                    n += stream.Read(bytes, n, length - n);
                res.Content = new ByteArrayContent(bytes);
            }
            else if (headers.ContainsKey("Transfer-Encoding"))
            {
                var encoding = headers["Transfer-Encoding"].First();
                if (string.Compare(encoding, "chunked", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        while (true)
                        {
                            string lengthString = ReadLine(stream);
                            if (string.IsNullOrEmpty(lengthString))
                                break;
                            int length = int.Parse(lengthString, NumberStyles.HexNumber);
                            if (length == 0)
                                break;
                            var bytes = new byte[length];
                            var n = 0;
                            while (n < length)
                                n += stream.Read(bytes, n, length - n);
                            ms.Write(bytes, 0, length);
                        }
                        ms.Position = 0;
                        res.Content = new ByteArrayContent(ms.ToArray());
                    }
                }
                else
                {
                    throw new NotSupportedException("Transfer-Encoding: " + encoding);
                }
            }

            return res;
        }

        private string ReadLine(Stream stream)
        {
            const byte CR = 0x0D;
            const byte LF = 0x0A;
            var flag = false;
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    var b = stream.ReadByte();
                    if (b == -1)
                        return null;
                    var bb = (byte)b;
                    ms.WriteByte(bb);
                    if (bb == CR)
                    {
                        flag = true;
                    }
                    else if (flag && (bb == LF))
                    {
                        ms.Position = 0;
                        var bytes = ms.ToArray();
                        return Encoding.ASCII.GetString(bytes, 0, (int)ms.Length - 2);
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }
        }

    }
}
