using System.Collections.Generic;
using Apex.Services.Enums;
using Apex.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.Controllers
{
    [Area("admin"), Authorize]
    public abstract class AdminController : Controller
    {
        protected const int DefaultPage = 1;
        protected const int DefaultSize = 25;

        protected IActionResult BadRequestApiError(string source, string message)
        {
            IEnumerable<ValidationError> errors = new List<ValidationError>
            {
                new ValidationError(source, message)
            };

            return BadRequest(new ApiError(ApiErrorCode.ValidateViewModelFail, errors));
        }

        //protected virtual DataTablesJsonResult DataTablesJson(
        //    DataTablesRequest request,
        //    int totalRecords,
        //    int totalRecordsFiltered,
        //    object data,
        //    bool allowJsonThroughHttpGet = true)
        //{
        //    var response = request.CreateResponse(totalRecords, totalRecordsFiltered, data);

        //    return DataTablesJson(response, allowJsonThroughHttpGet);
        //}

        //protected virtual DataTablesJsonResult DataTablesJson(DataTablesResponse response, bool allowJsonThroughHttpGet = true)
        //{
        //    return new DataTablesJsonResult(response, allowJsonThroughHttpGet);
        //}
    }
}
