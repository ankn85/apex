using System;
using System.Net;

namespace Apex.Services.Models
{
    public sealed class ApiException : Exception
    {
        public ApiException(
            string message,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public ApiException(
            Exception exception,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(exception.Message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}
