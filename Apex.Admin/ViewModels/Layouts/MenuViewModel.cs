using System.Collections.Generic;
using Apex.Data.Entities.Accounts;

namespace Apex.Admin.ViewModels.Layouts
{
    public sealed class MenuViewModel
    {
        public MenuViewModel(Menu menu)
        {
            Title = menu.Title;
            Url = string.IsNullOrEmpty(menu.Url) ? "javascript:;" : menu.Url;
            Icon = menu.Icon;
            SubMenus = new List<MenuViewModel>();
        }

        public string Title { get; }

        public string Url { get; }

        public string Icon { get; }

        public IList<MenuViewModel> SubMenus { get; set; }
    }
}
