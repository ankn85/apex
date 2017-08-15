using System;
using System.Net;
using System.Text;
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
            WriteLog(httpContext, exception, apiError.StatusCode);

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

        private void WriteLog(HttpContext httpContext, Exception exception, int eventId)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                sb = LogHttpContext(httpContext)
                    .Append(GetExceptionDetail(exception));
            }
            catch (Exception)
            {
            }

            _logger.LogError(new EventId(eventId), exception, sb.ToString());
        }

        private StringBuilder LogHttpContext(HttpContext httpContext)
        {
            StringBuilder sb = new StringBuilder(256);
            HttpRequest request = httpContext.Request;

            sb.Append("Url: ").Append(request.Path.Value).Append("<br/>");
            sb.Append("QueryString: ").Append(request.QueryString.ToString()).Append("<br/>");
            //sb.Append("Form: ").Append(request.Form.ToString()).Append("<br/>");
            sb.Append("Content Type: ").Append(request.ContentType).Append("<br/>");
            sb.Append("Content Length: ").Append(request.ContentLength).Append("<br/>");
            sb.Append("Remote IP: ").Append(httpContext.Connection.RemoteIpAddress.ToString());

            return sb;
        }

        private StringBuilder GetExceptionDetail(Exception exception)
        {
            StringBuilder sb = new StringBuilder(256);

            if (exception != null)
            {
                sb.Append("Source: ").Append(exception.Source).Append("<br/>")
                    .Append("Message: ").Append(exception.Message).Append("<br/>");
            }

            return sb;
        }
    }
}
