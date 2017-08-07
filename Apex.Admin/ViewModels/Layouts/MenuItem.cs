using System.Collections.Generic;
using Apex.Data.Entities.Accounts;

namespace Apex.Admin.ViewModels.Layouts
{
    public sealed class MenuItem
    {
        public MenuItem(Menu menu)
        {
            Title = menu.Title;
            Url = string.IsNullOrEmpty(menu.Url) ? "javascript:;" : menu.Url;
            Icon = menu.Icon;
            SubMenuItems = new List<MenuItem>();
        }

        public string Title { get; }

        public string Url { get; }

        public string Icon { get; }

        public IList<MenuItem> SubMenuItems { get; set; }
    }
}
