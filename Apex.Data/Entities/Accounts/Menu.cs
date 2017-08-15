using System.Collections.Generic;

namespace Apex.Data.Entities.Accounts
{
    public class Menu : BaseEntity
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public string Note { get; set; }

        public string NoteBackground { get; set; }

        public int Priority { get; set; }

        public ICollection<Menu> SubMenus { get; set; }

        public int? ParentId { get; set; }

        public virtual Menu ParentMenu { get; set; }

        public virtual ICollection<ApplicationRoleMenu> RoleMenus { get; set; }
    }
}
