using System.Collections.Generic;
using Apex.Admin.ViewModels.Layouts;
using Apex.Data.Entities.Accounts;
using Apex.Services.Enums;

namespace Apex.Admin.Models
{
    public interface IAdminContext
    {
        int UserId { get; }

        string UserName { get; }

        string Email { get; }

        MenuItem[] MenuItems { get; }

        Permission GetPermission(string url);

        bool HasRead(string url);

        bool HasHost(string url);

        void SetAdminContext(ApplicationUser user, IList<Menu> menus);
    }
}
