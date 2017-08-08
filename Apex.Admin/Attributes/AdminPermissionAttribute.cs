using System;
using System.Threading.Tasks;
using Apex.Admin.Models;
using Apex.Services.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Apex.Admin.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AdminPermissionAttribute : TypeFilterAttribute
    {
        public AdminPermissionAttribute(
            Permission permission,
            string action = null,
            string controller = null,
            string area = null)
            : base(typeof(AdminPermissionFilter))
        {
            Arguments = new object[] { permission, action, controller, area };
        }

        private class AdminPermissionFilter : IAsyncActionFilter
        {
            private readonly Permission _permission;
            private readonly string _action;
            private readonly string _controller;
            private readonly string _area;

            //private readonly IAdminContext _adminContext;

            public AdminPermissionFilter(
                //IAdminContext adminContext,
                Permission permission,
                string action = null,
                string controller = null,
                string area = null)
            {
                _permission = permission;
                _action = action;
                _controller = controller;
                _area = area;

                //_adminContext = adminContext;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var routedata = context.RouteData.Values;
                string action = _action ?? routedata["action"].ToString();
                string controller = _controller ?? routedata["controller"].ToString();
                string area = _area ?? routedata["area"].ToString();

                string url = $"/{area}/{controller}";

                if (!action.Equals("index", StringComparison.OrdinalIgnoreCase))
                {
                    url += $"/{action}";
                }

                //if (!_adminContext.GetPermission(url).HasFlag(_permission))
                //{
                //    throw new UnauthorizedAccessException();
                //}

                await next.Invoke();
            }
        }
    }

    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    //public class AdminPermissionAttribute : ActionFilterAttribute
    //{
    //    private readonly Permission _permission;
    //    private readonly string _action;
    //    private readonly string _controller;
    //    private readonly string _area;

    //    public AdminPermissionAttribute(
    //        Permission permission,
    //        string action = null,
    //        string controller = null,
    //        string area = null)
    //    {
    //        _permission = permission;
    //        _action = action;
    //        _controller = controller;
    //        _area = area;
    //    }

    //    public override void OnActionExecuting(ActionExecutingContext context)
    //    {
    //        var routedata = context.RouteData.Values;
    //        string action = _action ?? routedata["action"].ToString();
    //        string controller = _controller ?? routedata["controller"].ToString();
    //        string area = _area ?? routedata["area"].ToString();

    //        string url = $"/{area}/{controller}";

    //        if (!action.Equals("index", StringComparison.OrdinalIgnoreCase))
    //        {
    //            url += $"/{action}";
    //        }

    //        //if (!_adminContext.GetPermission(url).HasFlag(_permission))
    //        //{
    //        //    throw new UnauthorizedAccessException();
    //        //}

    //        base.OnActionExecuting(context);
    //    }
    //}
}
