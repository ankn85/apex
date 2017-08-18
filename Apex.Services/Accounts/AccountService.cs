using System;
using System.Collections.Generic;
using System.Linq;
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
            if (id <= 0)
            {
                return null;
            }

            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<IPagedList<ApplicationUserDto>> GetListAsync(
            string email,
            int[] roleIds,
            string sortColumnName,
            SortDirection sortDirection,
            int page,
            int size)
        {
            var query = _userManager.Users.AsNoTracking();
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

            var roleHash = await GetRolesHash();
            var pagedList = query
                .OrderBy(sortColumnName, sortDirection)
                .Skip(page)
                .Take(size)
                .Select(u => new ApplicationUserDto(u, roleHash));

            return PagedList<ApplicationUserDto>.Create(pagedList, totalRecords, totalRecordsFiltered);
        }

        public async Task<IdentityResult> CreateAsync(
            ApplicationUser entity,
            string password,
            bool locked,
            string[] roleNames)
        {
            var result = await _userManager.CreateAsync(entity, password);

            if (result.Succeeded && locked)
            {
                result = await LockAccountAsync(entity);
            }

            if (result.Succeeded && roleNames != null && roleNames.Length != 0)
            {
                result = await _userManager.AddToRolesAsync(entity, roleNames);
            }

            return result;
        }

        public async Task<IdentityResult> UpdateAsync(
            ApplicationUser entity,
            bool locked,
            string[] roleNames)
        {
            var result = await _userManager.UpdateAsync(entity);

            var isLockedOut = await _userManager.IsLockedOutAsync(entity);

            if (locked && !isLockedOut)
            {
                result = await LockAccountAsync(entity);
            }
            else if (!locked && isLockedOut)
            {
                result = await UnlockAccountAsync(entity);
            }

            // Assign Roles.
            var currentRoles = await _userManager.GetRolesAsync(entity);

            if (roleNames == null || roleNames.Length == 0)
            {
                result = await _userManager.RemoveFromRolesAsync(entity, currentRoles);
            }
            else
            {
                var allRoles = await _roleService.GetFromCacheAsync();
                var rolesExists = roleNames.Intersect(allRoles.Select(r => r.Name));

                if (rolesExists.Any())
                {
                    result = await _userManager.RemoveFromRolesAsync(entity, currentRoles);

                    if (result.Succeeded)
                    {
                        result = await _userManager.AddToRolesAsync(entity, rolesExists);
                    }
                }
            }

            return result;
        }

        public async Task<int> DeleteAsync(int[] ids)
        {
            ApplicationUser entity;
            IdentityResult result;
            int effectedRows = 0;

            foreach (int id in ids)
            {
                entity = await FindAsync(id);

                if (entity != null)
                {
                    result = await _userManager.DeleteAsync(entity);

                    if (result.Succeeded)
                    {
                        effectedRows++;
                    }
                }
            }

            return effectedRows;
        }

        public async Task<IdentityResult> LockAccountAsync(ApplicationUser entity)
        {
            return await _userManager.SetLockoutEndDateAsync(entity, DateTimeOffset.MaxValue);
        }

        public async Task<IdentityResult> UnlockAccountAsync(ApplicationUser entity)
        {
            var result = await _userManager.SetLockoutEndDateAsync(entity, null);

            if (result.Succeeded)
            {
                result = await _userManager.ResetAccessFailedCountAsync(entity);
            }

            return result;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser entity, string password)
        {
            var result = await _userManager.RemovePasswordAsync(entity);

            if (result.Succeeded)
            {
                result = await _userManager.AddPasswordAsync(entity, password);
            }

            return result;
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser entity)
        {
            return await _userManager.GetRolesAsync(entity);
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

        private async Task<IDictionary<int, string>> GetRolesHash()
        {
            var roles = await _roleService.GetFromCacheAsync();

            return roles.ToDictionary(r => r.Id, r => r.Name);
        }
    }
}
