using System.Collections.Generic;
using Apex.Data.Entities.Accounts;

namespace Apex.Services.Accounts
{
    public interface IPermissionProvider
    {
        IEnumerable<PermissionRecord> GetPermissionRecords();
    }
}
