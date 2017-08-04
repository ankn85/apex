using System.Collections.Generic;
using System.Linq;
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
            IQueryable<ApplicationUser> source,
            string email,
            int[] roleIds,
            string sortColumnName,
            SortDirection sortDirection,
            int page,
            int size);

        Task<IList<string>> GetRolesAsync(ApplicationUser entity);

        Task<IdentityResult> ResetPasswordAsync(ApplicationUser entity, string password);
    }
}
