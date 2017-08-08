using System.Threading.Tasks;
using Apex.Services.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.Controllers
{
    public class MenuController : AdminController
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IActionResult> Index()
        {
            var menus = await _menuService.GetListAsync();

            return View(menus);
        }
    }
}
