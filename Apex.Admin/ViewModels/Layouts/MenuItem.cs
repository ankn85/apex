using System.Collections.Generic;
using Apex.Data.Entities.Accounts;

namespace Apex.Admin.ViewModels.Layouts
{
    public sealed class MenuItem
    {
        public MenuItem()
        {
        }

        public MenuItem(Menu menu)
        {
            Title = menu.Title;
            Url = string.IsNullOrEmpty(menu.Url) ? "javascript:;" : menu.Url;
            Icon = menu.Icon;
            Note = menu.Note;
            NoteIcon = menu.NoteIcon;
            SubMenuItems = new List<MenuItem>();
        }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public string Note { get; set; }

        public string NoteIcon { get; set; }

        public bool Selected { get; set; }

        public IList<MenuItem> SubMenuItems { get; set; }
    }
}
