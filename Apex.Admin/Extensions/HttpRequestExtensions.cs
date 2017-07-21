using Microsoft.AspNetCore.Http;

namespace Apex.Admin.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string RequestedWithHeader = "X-Requested-With";
        private const string XmlHttpRequest = "XMLHttpRequest";

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            return request != null &&
                request.Headers != null &&
                request.Headers[RequestedWithHeader] == XmlHttpRequest;
        }
    }
}
