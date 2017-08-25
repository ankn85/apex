using Apex.Data;
using Apex.Data.Entities.Accounts;
using Apex.Data.Entities.Menus;
using Apex.Services.Menus;
using Apex.Services.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apex.Services.Accounts
{
    public class ACLService : BaseService<ApplicationRoleMenu>, IACLService
    {
        private readonly IRoleService _roleService;
        private readonly IMenuService _menuService;

        public ACLService(
            ObjectDbContext dbContext,
            IRoleService roleService,
            IMenuService menuService) : base(dbContext)
        {
            _roleService = roleService;
            _menuService = menuService;
        }

        public async Task<ACLDto> GetListAsync()
        {
            var roles = await _roleService.GetFromCacheAsync();
            var menus = await _menuService.GetListAsync();
            //var roleMenuDictionary = await Table.AsNoTracking()
            //    .ToDictionaryAsync(kvp => kvp.MenuId, kvp => kvp);

            //ApplicationRoleMenu applicationRoleMenu;
            //IList<ApplicationRoleMenuDto> roleMenus = new List<ApplicationRoleMenuDto>();

            //foreach (var menu in menus)
            //{
            //    if (roleMenuDictionary.TryGetValue(menu.Id, out applicationRoleMenu))
            //    {
            //        roleMenus.Add(new ApplicationRoleMenuDto(
            //            menu.Id,
            //            menu.Title,
            //            applicationRoleMenu.RoleId,
            //            applicationRoleMenu.Permission));
            //    }
            //    else
            //    {

            //    }
            //}

            return new ACLDto(roles, menus, null);
        }
    }
}
