using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Apex.Data.Entities.Accounts
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole()
            : base()
        {
        }

        public ApplicationRole(string roleName)
            : base(roleName)
        {
        }

        public virtual ICollection<PermissionRecordRole> PermissionRecordRoles { get; set; }
    }
}
