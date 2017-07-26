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
        protected IActionResult BadRequestApiError(string source, string message)
        {
            IEnumerable<ValidationError> errors = new List<ValidationError>
            {
                new ValidationError(source, message)
            };

            return BadRequest(new ApiError(ApiErrorCode.ValidateViewModelFail, errors));
        }
    }
}
