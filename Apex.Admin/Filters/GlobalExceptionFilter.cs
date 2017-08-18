using System;
using System.Net;
using Apex.Admin.Controllers;
using Apex.Admin.Extensions;
using Apex.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Apex.Admin.Filters
{
    public sealed class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            Exception exception = context.Exception;
            ApiError apiError = GetApiError(exception);

            HttpContext httpContext = context.HttpContext;
            _logger.LogError(new EventId(apiError.StatusCode), exception, exception.Message);

            HttpResponse response = httpContext.Response;
            response.StatusCode = apiError.StatusCode;

            context.ExceptionHandled = true;

            if (httpContext.Request.IsAjaxRequest())
            {
                response.ContentType = "application/json";
                context.Result = new JsonResult(apiError);
            }
            else
            {
                response.ContentType = "text/HTML";
                context.Result = new RedirectToActionResult(nameof(ErrorController.InternalServerError), "Error", null);
            }
        }

        private ApiError GetApiError(Exception exception)
        {
            Type exceptionType = exception.GetType();

            if (exceptionType == typeof(ApiException))
            {
                ApiException apiException = (ApiException)exception;

                return new ApiError(apiException.StatusCode, apiException.Message);
            }

            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                return new ApiError(HttpStatusCode.Unauthorized, "Unauthorized Access.");
            }

            Exception innerException = exceptionType == typeof(DbUpdateException) ?
                exception.InnerException :
                exception;

            return new ApiError(HttpStatusCode.InternalServerError, "An unhandled error occurred.");
        }
    }
}
