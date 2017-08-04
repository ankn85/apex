using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Apex.Data.Entities.Accounts;
using Apex.Data.Paginations;
using Apex.Data.Sorts;
using Apex.Services.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Apex.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly IRoleService _roleService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(
            IRoleService roleService,
            UserManager<ApplicationUser> userManager)
        {
            _roleService = roleService;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> FindAsync(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<IPagedList<ApplicationUserDto>> GetListAsync(
            IQueryable<ApplicationUser> source,
            string email,
            int[] roleIds,
            string sortColumnName,
            SortDirection sortDirection,
            int page,
            int size)
        {
            var query = source.AsNoTracking();
            int totalRecords = await query.CountAsync();

            if (totalRecords == 0)
            {
                return PagedList<ApplicationUserDto>.Empty();
            }

            query = GetFilterList(query, email, roleIds);
            int totalRecordsFiltered = await query.CountAsync();

            if (totalRecordsFiltered == 0)
            {
                return PagedList<ApplicationUserDto>.Empty(totalRecords);
            }

            var roleHash = await GetRoleHash();

            return PagedList<ApplicationUserDto>.Create(
                GetSortAndPagedList(query, sortColumnName, sortDirection, page, size).Select(u => new ApplicationUserDto(u, roleHash)),
                totalRecords,
                totalRecordsFiltered);
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser entity)
        {
            return await _userManager.GetRolesAsync(entity);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser entity, string password)
        {
            //ApplicationUser entity = await FindAsync(id);

            //if (entity == null)
            //{
            //    throw new ApiException(
            //        $"{nameof(ApplicationUser)} not found. Id = {id}",
            //        ApiErrorCode.NotFound);
            //}

            var result = await _userManager.RemovePasswordAsync(entity);

            if (result.Succeeded)
            {
                result = await _userManager.AddPasswordAsync(entity, password);
            }
            
            return result;
        }

        private IQueryable<ApplicationUser> GetFilterList(
            IQueryable<ApplicationUser> source,
            string email,
            int[] roleIds)
        {
            source = source.Include(u => u.Roles);

            if (!string.IsNullOrEmpty(email))
            {
                email = email.ToUpperInvariant();

                source = source.Where(u => u.NormalizedEmail.Contains(email));
            }

            if (roleIds != null && roleIds.Length > 0)
            {
                source = source.Where(u => u.Roles.Any(r => roleIds.Contains(r.RoleId)));
            }

            return source;
        }

        private IEnumerable<ApplicationUser> GetSortAndPagedList(
            IQueryable<ApplicationUser> source,
            string sortColumnName,
            SortDirection sortDirection,
            int page,
            int size)
        {
            PropertyInfo property = typeof(ApplicationUser).GetProperties()
                .FirstOrDefault(p => p.Name.Equals(sortColumnName, StringComparison.OrdinalIgnoreCase));

            var sortList = sortDirection == SortDirection.Ascending ?
                source.OrderBy(property.GetValue) :
                source.OrderByDescending(property.GetValue);

            return sortList.Skip(page).Take(size);
        }

        private async Task<IDictionary<int, string>> GetRoleHash()
        {
            var roles = await _roleService.GetListAsync();

            return roles.ToDictionary(r => r.Id, r => r.Name);
        }
    }
}
