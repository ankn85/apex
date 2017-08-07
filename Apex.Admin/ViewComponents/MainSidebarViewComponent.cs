using System.Collections.Generic;
using System.Threading.Tasks;
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

        public MainSidebarViewComponent(
            UserManager<ApplicationUser> userManager,
            IMenuService menuService)
        {
            _userManager = userManager;
            _menuService = menuService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IList<MenuItem> menuViewModels = await GetMenusAsync();

            return View(menuViewModels);
        }

        private async Task<IList<MenuItem>> GetMenusAsync()
        {
            IList<MenuItem> menuViewModels = new List<MenuItem>();
            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                var menus = await _menuService.GetReadListAsync(user);

                foreach (var menu in menus)
                {
                    if (menu.ParentId == null || menu.ParentId.Value == 0)
                    {
                        GetMenuViewModels(menu, menuViewModels);
                    }
                }
            }

            return menuViewModels;
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            int id;
            string userId = _userManager.GetUserId(HttpContext.User);

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out id))
            {
                return null;
            }

            return await _userManager.Users
                .Include(u => u.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        private void GetMenuViewModels(Menu menu, IList<MenuItem> menuViewModels)
        {
            MenuItem viewModel = new MenuItem(menu);
            menuViewModels.Add(viewModel);

            foreach (var sub in menu.SubMenus)
            {
                GetMenuViewModels(sub, viewModel.SubMenuItems);
            }
        }
    }
}
