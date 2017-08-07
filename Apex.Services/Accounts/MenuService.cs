using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities.Accounts;
using Apex.Services.Enums;
using Microsoft.EntityFrameworkCore;

namespace Apex.Services.Accounts
{
    public class MenuService : BaseService<Menu>, IMenuService
    {
        public MenuService(ObjectDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IList<Menu>> GetReadListAsync(ApplicationUser user)
        {
            var roleIds = user.Roles.Select(role => role.RoleId);

            return await Table
                .Include(m => m.SubMenus)
                .Include(m => m.RoleMenus)
                .Where(m =>
                    //(m.ParentId == null || m.ParentId.Value == 0) &&
                    m.RoleMenus.Any(rm => roleIds.Contains(rm.RoleId) &&
                    ((Permission)rm.Permission).HasFlag(Permission.Read)))
                .OrderBy(m => m.Priority)
                .ToListAsync();
        }
    }
}
