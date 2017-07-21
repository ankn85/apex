using System.Collections.Generic;

namespace Apex.Data.Entities.Accounts
{
    public class PermissionRecord : BaseEntity
    {
        public string Name { get; set; }

        public string SystemName { get; set; }

        public string Category { get; set; }

        public virtual ICollection<PermissionRecordRole> PermissionRecordRoles { get; set; }
    }
}
