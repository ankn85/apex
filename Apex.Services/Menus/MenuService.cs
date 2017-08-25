using Apex.Data;
using Apex.Data.Entities.Menus;
using Apex.Services.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apex.Services.Menus
{
    public class MenuService : BaseService<Menu>, IMenuService
    {
        public MenuService(ObjectDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<Menu>> GetListAsync()
        {
            var menus = await Table
                .Include(m => m.SubMenus)
                .OrderBy(m => m.Priority)
                .AsNoTracking()
                .ToListAsync();

            return menus.Where(m => m.ParentId == null || m.ParentId == 0);
        }

        public async Task<IList<Menu>> GetReadListAsync(IEnumerable<int> roleIds)
        {
            return await Table
                .Include(m => m.SubMenus)
                .Include(m => m.RoleMenus)
                .Where(m =>
                    m.RoleMenus.Any(rm => roleIds.Contains(rm.RoleId) &&
                    ((Permission)rm.Permission).HasFlag(Permission.Read)))
                .OrderBy(m => m.Priority)
                .ToListAsync();
        }

        public override async Task<Menu> CreateAsync(Menu entity)
        {
            await UpdateActiveDependOnParentAsync(entity);

            return await base.CreateAsync(entity);
        }

        public async Task<int> UpdateAsync(Menu entity)
        {
            await Task.WhenAll(
                UpdateActiveDependOnParentAsync(entity),
                UpdateInactiveAllSubMenusAsync(entity));

            return await CommitAsync();
        }

        private async Task<Menu> UpdateActiveDependOnParentAsync(Menu entity)
        {
            if (entity.ParentId != null && entity.Active)
            {
                Menu parent = await FindAsync(entity.ParentId.Value);

                if (parent != null)
                {
                    entity.Active = parent.Active;
                }
            }

            return entity;
        }

        private async Task UpdateInactiveAllSubMenusAsync(Menu entity)
        {
            if (!entity.Active)
            {
                var subMenus = await Table
                    .Include(m => m.SubMenus)
                    .Where(m => m.ParentId == entity.Id)
                    .ToListAsync();

                UpdateInactiveAllSubMenusAsync(subMenus);
            }
        }

        private void UpdateInactiveAllSubMenusAsync(ICollection<Menu> menus)
        {
            foreach (var menu in menus)
            {
                menu.Active = false;

                if (menu.SubMenus?.Count > 0)
                {
                    UpdateInactiveAllSubMenusAsync(menu.SubMenus);
                }
            }
        }
    }
}
