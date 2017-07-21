using System.Linq;
using Apex.Admin.Extensions;
using Apex.Services.Enums;
using Apex.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Apex.Admin.Attributes
{
    public sealed class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new NotFoundResult();
            }

            if (!filterContext.ModelState.IsValid)
            {
                var errors = from kvp in filterContext.ModelState
                             from e in kvp.Value.Errors
                             let k = kvp.Key
                             select new ValidationError(k, e.ErrorMessage);

                filterContext.Result = new BadRequestObjectResult(new ApiError(ApiErrorCode.ValidateViewModelFail, errors));
            }
        }
    }
}
