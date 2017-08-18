using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Data.Entities.Accounts;
using Apex.Data.Paginations;
using Apex.Data.Sorts;
using Apex.Services.Models;
using Microsoft.AspNetCore.Identity;

namespace Apex.Services.Accounts
{
    public interface IRoleService
    {
        Task<ApplicationRole> FindAsync(int id);

        Task<IPagedList<ApplicationRole>> GetListAsync(
            string sortColumnName,
            SortDirection sortDirection);

        Task<IList<IdNameDto>> GetFromCacheAsync();

        Task<bool> RoleExistsAsync(string roleName);

        Task<IdentityResult> CreateAsync(ApplicationRole entity);

        Task<IdentityResult> UpdateAsync(ApplicationRole entity);

        Task<int> DeleteAsync(int[] ids);
    }
}
