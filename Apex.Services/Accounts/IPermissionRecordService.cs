using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Data.Entities.Accounts;

namespace Apex.Services.Accounts
{
    public interface IPermissionRecordService
    {
        Task<IEnumerable<PermissionRecord>> GetPermissionRecordsByUser(ApplicationUser user);
    }
}
