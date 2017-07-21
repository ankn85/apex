using System.Collections.Generic;
using Apex.Services.Enums;

namespace Apex.Admin.Models
{
    public class WorkingContext
    {
        public WorkingContext(int userId, string userName, string email, IDictionary<string, Permission> permissions)
        {
            UserId = userId;
            UserName = userName;
            Email = email;
            Permissions = permissions;
        }

        public int UserId { get; }

        public string UserName { get; }

        public string Email { get; }

        public IDictionary<string, Permission> Permissions { get; }
    }
}
