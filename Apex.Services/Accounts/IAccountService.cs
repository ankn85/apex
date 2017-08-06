using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Data.Entities.Accounts;
using Apex.Data.Paginations;
using Apex.Data.Sorts;
using Apex.Services.Models.Accounts;
using Microsoft.AspNetCore.Identity;

namespace Apex.Services.Accounts
{
    public interface IAccountService
    {
        Task<ApplicationUser> FindAsync(int id);

        Task<IPagedList<ApplicationUserDto>> GetListAsync(
            string email,
            int[] roleIds,
            string sortColumnName,
            SortDirection sortDirection,
            int page,
            int size);

        Task<IdentityResult> CreateAsync(
            ApplicationUser entity,
            string password,
            bool locked,
            string[] roleNames);

        Task<IdentityResult> UpdateAsync(
            ApplicationUser entity,
            bool locked,
            string[] roleNames);

        Task<int> DeleteAsync(int[] ids);

        Task<IdentityResult> LockAccountAsync(ApplicationUser entity);

        Task<IdentityResult> UnlockAccountAsync(ApplicationUser entity);

        Task<IdentityResult> ResetPasswordAsync(ApplicationUser entity, string password);

        Task<IList<string>> GetRolesAsync(ApplicationUser entity);
    }
}
