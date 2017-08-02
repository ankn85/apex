using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Data.Entities.Accounts;
using Apex.Data.Paginations;
using Apex.Services.Enums;
using Apex.Services.Models.Accounts;
using Microsoft.AspNetCore.Identity;

namespace Apex.Services.Accounts
{
    public interface IAccountService
    {
        Task<ApplicationUser> FindAsync(int id);

        Task<IPagedList<ApplicationUserDto>> GetListAsync(
            string email,
            IList<int> roleIds,
            string sortColumnName,
            SortDirection sortDirection,
            int page,
            int size);

        Task<IList<string>> GetRolesAsync(ApplicationUser entity);

        Task<IdentityResult> CreateAsync(ApplicationUser entity, string password, bool locked, IList<string> roleNames);

        Task<IdentityResult> UpdateAsync(ApplicationUser entity, bool locked, IList<string> roleNames);

        Task<IdentityResult> DeleteAsync(int id);
    }
}
