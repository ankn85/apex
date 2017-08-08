using Apex.Admin.Models;
using Apex.Admin.ViewModels.Layouts;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.ViewComponents
{
    public sealed class MainSidebarViewComponent : ViewComponent
    {
        private readonly IAdminContext _adminContext;

        public MainSidebarViewComponent(IAdminContext adminContext)
        {
            _adminContext = adminContext;
        }

        public IViewComponentResult Invoke()
        {
            return View(new MainSidebar(_adminContext.FullName, _adminContext.MenuItems));
        }
    }
}
