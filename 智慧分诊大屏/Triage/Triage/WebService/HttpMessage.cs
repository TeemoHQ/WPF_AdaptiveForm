using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WaitingRoomBigScreen.WebService
{
    public class HttpMessage
    {
        public Dictionary<string, List<string>> Headers { get; } =
            new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        public HttpContent Content { get; set; }
    }

    public class HttpRequest : HttpMessage
    {
        public HttpMethod Method { get; set; }

        public Uri RequestUri { get; set; }
    }

    public class HttpResponse : HttpMessage
    {
        public HttpResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccessStatusCode => StatusCode == HttpStatusCode.OK;
    }
}
