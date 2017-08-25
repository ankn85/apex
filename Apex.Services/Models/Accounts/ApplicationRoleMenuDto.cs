using Apex.Services.Enums;

namespace Apex.Services.Models.Accounts
{
    public sealed class ApplicationRoleMenuDto
    {
        public ApplicationRoleMenuDto(
            int menuId,
            string title,
            int roleId,
            int permission)
        {
            MenuId = menuId;
            Title = title;
            RoleId = roleId;

            Permission per = (Permission)permission;
            Read = per.HasFlag(Permission.Read);
            Full = per.HasFlag(Permission.Full);
        }

        public int MenuId { get; }

        public string Title { get; }

        public int RoleId { get; }

        public bool Read { get; }

        public bool Full { get; }
    }
}
