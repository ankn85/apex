using System.Collections.Generic;
using System.Linq;
using Apex.Admin.Extensions;
using Apex.Admin.ViewModels.Layouts;
using Apex.Data.Entities.Accounts;
using Apex.Services.Enums;
using Microsoft.AspNetCore.Http;

namespace Apex.Admin.Models
{
    public sealed class AdminContext : IAdminContext
    {
        private const string UserIdSessionKey = "UserId";
        private const string UserNameSessionKey = "UserName";
        private const string EmailSessionKey = "Email";
        private const string MenuItemsSessionKey = "MenuItems";

        private readonly ISession _session;

        public AdminContext(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext.Session;
        }

        public int UserId
        {
            get
            {
                return _session.GetInt32(UserIdSessionKey) ?? 0;
            }
            private set
            {
                _session.SetInt32(UserIdSessionKey, value);
            }
        }

        public string UserName
        {
            get
            {
                return _session.GetString(UserNameSessionKey);
            }
            private set
            {
                _session.SetString(UserNameSessionKey, value);
            }
        }

        public string Email
        {
            get
            {
                return _session.GetString(EmailSessionKey);
            }
            private set
            {
                _session.SetString(EmailSessionKey, value);
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
            UserId = user.Id;
            UserName = user.UserName;
            Email = user.Email;

            IList<MenuItem> menuItems = new List<MenuItem>();
            var rootMenus = menus.Where(m => m.ParentId == null || m.ParentId.Value == 0);

            foreach (var item in rootMenus)
            {
                MenuItem menuItem = new MenuItem(item);
                BuildMenuItems(item.ParentId, menuItem, menus);

                menuItems.Add(menuItem);
            }
        }

        private void BuildMenuItems(int? parentId, MenuItem menuItem, IList<Menu> allMenus)
        {
            var subMenus = allMenus.Where(m => m.ParentId == parentId);

            foreach (var item in subMenus)
            {
                MenuItem subMenuItem = new MenuItem(item);
                BuildMenuItems(item.ParentId, subMenuItem, allMenus);
            }
        }
    }
}
