using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.ViewModels.DataTables
{
    public sealed class DataTablesJsonResult : IActionResult
    {
        private const bool AllowJsonThroughHttpGet = false;
        private const string DefaultContentType = "application/json; charset={0}";
        private static readonly Encoding DefaultContentEncoding = Encoding.UTF8;

        private readonly string _contentType;
        private readonly Encoding _contentEncoding;
        private readonly bool _allowGet;
        private readonly object _data;

        public DataTablesJsonResult(DataTablesResponse response)
            : this(response, DefaultContentType, DefaultContentEncoding, AllowJsonThroughHttpGet)
        {
        }

        public DataTablesJsonResult(DataTablesResponse response, bool allowJsonThroughHttpGet)
            : this(response, DefaultContentType, DefaultContentEncoding, allowJsonThroughHttpGet)
        {
        }

        public DataTablesJsonResult(
            DataTablesResponse response,
            string contentType,
            Encoding contentEncoding,
            bool allowJsonThroughHttpGet)
        {
            _contentEncoding = contentEncoding ?? Encoding.UTF8;
            _contentType = string.Format(contentType ?? DefaultContentType, _contentEncoding.WebName);
            _allowGet = allowJsonThroughHttpGet;

            _data = response;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var httpContext = context.HttpContext;

            if (!_allowGet && context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException(
                    "This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.");
            }

            var response = httpContext.Response;
            response.ContentType = _contentType;

            if (_data != null)
            {
                var content = _data.ToString();
                var contentBytes = _contentEncoding.GetBytes(content);

                await response.Body.WriteAsync(contentBytes, 0, contentBytes.Length);
            }
        }
    }
}
