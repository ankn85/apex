using System.Collections.Generic;
using System.Net;

namespace Apex.Services.Models
{
    public sealed class ApiError
    {
        public ApiError(HttpStatusCode statusCode, string errorMessage)
            : this(statusCode, errorMessage, null)
        {
        }

        public ApiError(HttpStatusCode statusCode, IEnumerable<ValidationError> validationErrors)
            : this(statusCode, null, validationErrors)
        {
        }

        private ApiError(
            HttpStatusCode statusCode,
            string errorMessage,
            IEnumerable<ValidationError> validationErrors)
        {
            StatusCode = (int)statusCode;
            ErrorMessage = errorMessage;
            ValidationErrors = validationErrors;
        }

        public int StatusCode { get; }

        public string ErrorMessage { get; }

        public IEnumerable<ValidationError> ValidationErrors { get; }
    }
}
