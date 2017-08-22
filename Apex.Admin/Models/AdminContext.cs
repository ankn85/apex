using System.Collections.Generic;
using System.Linq;
using Apex.Admin.Extensions;
using Apex.Admin.ViewModels.Layouts;
using Apex.Data.Entities.Accounts;
using Apex.Data.Entities.Menus;
using Apex.Services.Enums;
using Microsoft.AspNetCore.Http;

namespace Apex.Admin.Models
{
    public sealed class AdminContext : IAdminContext
    {
        private const string FullNameSessionKey = "FullName";
        private const string MenuItemsSessionKey = "MenuItems";

        private readonly ISession _session;

        public AdminContext(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext.Session;
        }

        public bool SessionCompleted => !string.IsNullOrEmpty(FullName) && MenuItems != null;

        public string FullName
        {
            get
            {
                return _session.GetString(FullNameSessionKey);
            }
            private set
            {
                _session.SetString(FullNameSessionKey, value);
            }
        }

        public MenuItem[] MenuItems
        {
            get
            {
                return _session.Get<MenuItem[]>(MenuItemsSessionKey);
            }
            private set
            {
                _session.Set(MenuItemsSessionKey, value);
            }
        }

        public Permission GetPermission(string url)
        {
            int value = _session.GetInt32(url) ?? 0;

            return (Permission)value;
        }

        public bool HasRead(string url)
        {
            return GetPermission(url).HasFlag(Permission.Read);
        }

        public bool HasHost(string url)
        {
            return GetPermission(url).HasFlag(Permission.Host);
        }

        public void SetAdminContext(ApplicationUser user, IList<Menu> menus)
        {
            FullName = user.FullName;

            SetMenuItemsAndPermissions(menus);
        }

        private void SetMenuItemsAndPermissions(IList<Menu> menus)
        {
            IList<MenuItem> menuItems = new List<MenuItem>();

            foreach (Menu menu in menus)
            {
                if (menu.ParentId == null || menu.ParentId.Value == 0)
                {
                    BuildMenuItems(menu, menuItems);
                }

                if (!string.IsNullOrEmpty(menu.Url) && menu.RoleMenus.Any())
                {
                    _session.SetInt32(menu.Url, menu.RoleMenus.Max(rm => rm.Permission));
                }
            }

            MenuItems = menuItems.ToArray();
        }

        private void BuildMenuItems(Menu menu, IList<MenuItem> menuItems)
        {
            MenuItem menuItem = new MenuItem(menu);
            menuItems.Add(menuItem);

            if (menu.SubMenus != null)
            {
                foreach (var sub in menu.SubMenus)
                {
                    BuildMenuItems(sub, menuItem.SubMenuItems);
                }
            }
        }

        public void Clear()
        {
            _session.Clear();
        }
    }
}
