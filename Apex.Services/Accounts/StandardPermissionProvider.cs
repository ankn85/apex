using System.Collections.Generic;
using Apex.Data.Entities.Accounts;

namespace Apex.Services.Accounts
{
    public class StandardPermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord ManageLogs = new PermissionRecord { Name = "Admin area. Manage Logs", SystemName = "ManageLogs", Category = "Systems" };
        public static readonly PermissionRecord ManageAccounts = new PermissionRecord { Name = "Admin area. Manage Accounts", SystemName = "ManageAccounts", Category = "Accounts" };

        public IEnumerable<PermissionRecord> GetPermissionRecords()
        {
            return new List<PermissionRecord>
            {
                ManageLogs,
                ManageAccounts
            };
        }
    }
}
