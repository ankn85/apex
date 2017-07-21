using System.Collections.Generic;
using Apex.Services.Enums;

namespace Apex.Services.Models
{
    public sealed class ApiError
    {
        public ApiError(ApiErrorCode errorCode, string errorMessage)
            : this(errorCode, errorMessage, null)
        {
        }

        public ApiError(ApiErrorCode errorCode, IEnumerable<ValidationError> validationErrors)
            : this(errorCode, null, validationErrors)
        {
        }

        private ApiError(
            ApiErrorCode errorCode,
            string errorMessage,
            IEnumerable<ValidationError> validationErrors)
        {
            ErrorCode = (int)errorCode;
            ErrorMessage = errorMessage;
            ValidationErrors = validationErrors;
        }

        public int ErrorCode { get; }

        public string ErrorMessage { get; }

        public IEnumerable<ValidationError> ValidationErrors { get; }
    }
}
