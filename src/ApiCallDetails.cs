using System.Net;
using System.Net.Http.Headers;

namespace Ivvy.API
{
    public class ApiCallDetails
    {
        public string RequestBody
        {
            get; set;
        }

        public string ResponseBody
        {
            get; set;
        }

        public HttpHeaders RequestHeaders
        {
            get; set;
        }

        public HttpHeaders ResponseHeaders
        {
            get; set;
        }

        public HttpStatusCode StatusCode
        {
            get; set;
        }
    }
}
