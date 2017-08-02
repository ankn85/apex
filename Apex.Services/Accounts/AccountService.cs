using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Apex.Data.Entities.Accounts;
using Apex.Data.Paginations;
using Apex.Services.Enums;
using Apex.Services.Models;
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
            string email,
            IList<int> roleIds,
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

            var roleHash = await GetRoleHash();

            return PagedList<ApplicationUserDto>.Create(
                GetSortAndPagedList(query, sortColumnName, sortDirection, page, size).Select(u => ToApplicationUserDto(u, roleHash)),
                totalRecords,
                totalRecordsFiltered);
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser entity)
        {
            return await _userManager.GetRolesAsync(entity);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser entity, string password, bool locked, IList<string> roleNames)
        {
            var result = await _userManager.CreateAsync(entity, password);

            if (result.Succeeded && locked)
            {
                ApplicationUser lockedEntity = await _userManager.FindByIdAsync(entity.Id.ToString());

                result = await LockAccount(lockedEntity);
            }

            if (result.Succeeded && roleNames != null && roleNames.Count != 0)
            {
                result = await _userManager.AddToRolesAsync(entity, roleNames);
            }

            return result;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser entity, bool locked, IList<string> roleNames)
        {
            ApplicationUser updatedEntity = await _userManager.FindByIdAsync(entity.Id.ToString());

            if (updatedEntity == null)
            {
                throw new ApiException(
                    $"{nameof(ApplicationUser)} not found. Id = {entity.Id}",
                    ApiErrorCode.NotFound);
            }

            updatedEntity.FullName = entity.FullName;
            updatedEntity.Gender = entity.Gender;
            updatedEntity.Birthday = entity.Birthday;
            updatedEntity.PhoneNumber = entity.PhoneNumber;
            updatedEntity.Address = entity.Address;

            IdentityResult result = await _userManager.UpdateAsync(updatedEntity);

            var isLockedOut = await _userManager.IsLockedOutAsync(updatedEntity);

            if (locked && !isLockedOut)
            {
                result = await LockAccount(updatedEntity);
            }
            else if (!locked && isLockedOut)
            {
                result = await UnlockAccount(updatedEntity);
            }

            // Assign Roles.
            var currentRoles = await _userManager.GetRolesAsync(updatedEntity);
            
            if (roleNames == null || roleNames.Count == 0)
            {
                result = await _userManager.RemoveFromRolesAsync(updatedEntity, currentRoles);
            }
            else
            {
                var allRoles = await _roleService.GetListAsync();
                var rolesNotExists = roleNames.Except(allRoles.Select(r => r.Name));

                if (rolesNotExists.Any())
                {
                    throw new ApiException(
                        $"Roles '{string.Join(",", rolesNotExists)}' does not exist in the system.",
                        ApiErrorCode.NotFound);
                }

                result = await _userManager.RemoveFromRolesAsync(updatedEntity, currentRoles);

                if (result.Succeeded)
                {
                    result = await _userManager.AddToRolesAsync(updatedEntity, roleNames);
                }
            }

            return result;
        }

        public async Task<IdentityResult> ResetPasswordAsync(int id, string password)
        {
            ApplicationUser entity = await _userManager.FindByIdAsync(id.ToString());

            if (entity == null)
            {
                throw new ApiException(
                    $"{nameof(ApplicationUser)} not found. Id = {id}",
                    ApiErrorCode.NotFound);
            }

            var result = await _userManager.RemovePasswordAsync(entity);

            if (result.Succeeded)
            {
                result = await _userManager.AddPasswordAsync(entity, password);
            }
            
            return result;
        }

        public async Task<IdentityResult> DeleteAsync(int id)
        {
            ApplicationUser entity = await _userManager.FindByIdAsync(id.ToString());

            if (entity == null)
            {
                throw new ApiException(
                    $"{nameof(ApplicationUser)} not found. Id = {id}",
                    ApiErrorCode.NotFound);
            }

            return await _userManager.DeleteAsync(entity);
        }

        private IQueryable<ApplicationUser> GetFilterList(
            IQueryable<ApplicationUser> source,
            string email,
            IList<int> roleIds)
        {
            source = source.Include(u => u.Roles);

            if (!string.IsNullOrEmpty(email))
            {
                email = email.ToUpperInvariant();

                source = source.Where(u => u.NormalizedEmail.Contains(email));
            }

            if (roleIds != null && roleIds.Count > 0)
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

        private ApplicationUserDto ToApplicationUserDto(ApplicationUser entity, IDictionary<int, string> roleHash)
        {
            return new ApplicationUserDto(
                entity.Id,
                entity.Email,
                entity.EmailConfirmed,
                entity.AccessFailedCount,
                entity.Roles.Select(r => r.RoleId),
                entity.LockoutEnabled,
                entity.LockoutEnd,
                entity.FullName,
                entity.Gender,
                entity.Birthday,
                entity.PhoneNumber,
                entity.Address,
                roleHash);
        }

        private async Task<IdentityResult> LockAccount(ApplicationUser entity, int? forDays = null)
        {
            DateTimeOffset lockoutEnd = forDays.HasValue ?
                DateTimeOffset.UtcNow.AddDays(forDays.Value) :
                DateTimeOffset.MaxValue;

            return await _userManager.SetLockoutEndDateAsync(entity, lockoutEnd);
        }

        private async Task<IdentityResult> UnlockAccount(ApplicationUser entity)
        {
            var result = await _userManager.SetLockoutEndDateAsync(entity, null);

            if (result.Succeeded)
            {
                result = await _userManager.ResetAccessFailedCountAsync(entity);
            }

            return result;
        }
    }
}
