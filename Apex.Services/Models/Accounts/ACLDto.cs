using Apex.Data.Entities.Menus;
using System.Collections.Generic;

namespace Apex.Services.Models.Accounts
{
    public sealed class ACLDto
    {
        public ACLDto(
            IEnumerable<IdNameDto> roles,
            IEnumerable<Menu> menus,
            IEnumerable<ApplicationRoleMenuDto> roleMenus)
        {
            Roles = roles;
            Menus = menus;
            RoleMenus = roleMenus;
        }

        public IEnumerable<IdNameDto> Roles { get; }

        public IEnumerable<Menu> Menus { get; }

        public IEnumerable<ApplicationRoleMenuDto> RoleMenus { get; }
    }
}
