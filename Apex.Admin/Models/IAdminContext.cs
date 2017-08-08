using System.Collections.Generic;
using Apex.Admin.ViewModels.Layouts;
using Apex.Data.Entities.Accounts;
using Apex.Services.Enums;

namespace Apex.Admin.Models
{
    public interface IAdminContext
    {
        string FullName { get; }

        MenuItem[] MenuItems { get; }

        Permission GetPermission(string url);

        bool HasRead(string url);

        bool HasHost(string url);

        void SetAdminContext(ApplicationUser user, IList<Menu> menus);
    }
}
