using System;

namespace Apex.Data.Entities.Accounts
{
    public class ApplicationRoleMenu : BaseEntity
    {
        public int RoleId { get; set; }

        public virtual ApplicationRole Role { get; set; }

        public int MenuId { get; set; }

        public virtual Menu Menu { get; set; }

        public int Permission { get; set; }

        public DateTime? CreatedOnUtc { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }
    }
}
