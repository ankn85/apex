//using System;
//using System.Security.Claims;
//using System.Security.Principal;
//using System.Threading.Tasks;
//using Apex.Data.Entities.Accounts;
//using Apex.Services.Enums;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.Logging;

//namespace Apex.Admin.Attributes
//{
//    public class AppPrincipal : ClaimsPrincipal
//    {
//        //private readonly IPermissionProvider _PermissionProvider;

//        public AppPrincipal(IPermissionProvider permissionProvider, IIdentity ntIdentity)
//            : base((ClaimsIdentity)ntIdentity)
//        {
//            _PermissionProvider = permissionProvider;
//        }

//        public override bool IsInRole(string role)
//        {
//            return _PermissionProvider.IsUserAuthorized(this, role);
//        }
//    }

//    public class PermissionsAuthorizationRequirement : IAuthorizationRequirement
//    {
//        public PermissionsAuthorizationRequirement(
//            PermissionRecord permissionRecord,
//            Permission requiredPermission)
//        {
//            RequiredPermission = requiredPermission;
//        }

//        public PermissionRecord PermissionRecord { get; }

//        public Permission RequiredPermission { get; }
//    }

//    public sealed class RequiresPermissionAttribute : TypeFilterAttribute
//    {
//        public RequiresPermissionAttribute(
//            PermissionRecord permissionRecord,
//            Permission requiredPermission)
//            : base(typeof(RequiresPermissionAttributeImplement))
//        {
//            Arguments = new[] { new PermissionsAuthorizationRequirement(permissionRecord, requiredPermission) };
//        }

//        private class RequiresPermissionAttributeImplement : Attribute, IAsyncResourceFilter
//        {
//            private readonly IAuthorizationService _authService;
//            private readonly PermissionsAuthorizationRequirement _requiredPermissions;

//            public RequiresPermissionAttributeImplement(
//                ILogger<RequiresPermissionAttribute> logger,
//                IAuthorizationService authService,
//                PermissionsAuthorizationRequirement requiredPermissions)
//            {
//                _authService = authService;
//                _requiredPermissions = requiredPermissions;
//            }

//            public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
//            {
//                if (!await _authService.AuthorizeAsync(
//                    context.HttpContext.User,
//                    context.ActionDescriptor.ToString(),
//                    _requiredPermissions))
//                {
//                    context.Result = new ChallengeResult();
//                }

//                await next();
//            }
//        }
//    }
//}
