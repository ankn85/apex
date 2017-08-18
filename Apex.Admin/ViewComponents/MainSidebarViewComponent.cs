using System;
using System.Linq;
using System.Threading.Tasks;
using Apex.Admin.Models;
using Apex.Admin.ViewModels.Layouts;
using Apex.Data.Entities.Accounts;
using Apex.Services.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apex.Admin.ViewComponents
{
    public sealed class MainSidebarViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMenuService _menuService;
        private readonly IAdminContext _adminContext;

        public MainSidebarViewComponent(
            UserManager<ApplicationUser> userManager,
            IMenuService menuService,
            IAdminContext adminContext)
        {
            _userManager = userManager;
            _menuService = menuService;
            _adminContext = adminContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_adminContext.SessionCompleted)
            {
                var user = await GetCurrentUserAsync();

                if (user != null)
                {
                    var menus = await _menuService.GetReadListAsync(user.Roles.Select(r => r.RoleId));

                    _adminContext.SetAdminContext(user, menus);
                }
            }

            return View(new MainSidebar(_adminContext.FullName, _adminContext.MenuItems));
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var principal = HttpContext.User;

            if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
            {
                return null;
            }

            return await _userManager.Users
                .Include(u => u.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.Equals(principal.Identity.Name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
