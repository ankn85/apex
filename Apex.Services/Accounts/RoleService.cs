using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apex.Data.Entities.Accounts;
using Apex.Data.Paginations;
using Apex.Data.Sorts;
using Apex.Services.Caching;
using Apex.Services.Constants;
using Apex.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Apex.Services.Accounts
{
    public class RoleService : IRoleService
    {
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleService(
            IMemoryCacheService memoryCacheService,
            RoleManager<ApplicationRole> roleManager)
        {
            _memoryCacheService = memoryCacheService;
            _roleManager = roleManager;
        }

        public async Task<ApplicationRole> FindAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return await _roleManager.FindByIdAsync(id.ToString());
        }

        public async Task<IPagedList<ApplicationRole>> GetListAsync(
            string sortColumnName,
            SortDirection sortDirection)
        {
            var query = _roleManager.Roles.AsNoTracking();
            int totalRecords = await query.CountAsync();

            if (totalRecords == 0)
            {
                return PagedList<ApplicationRole>.Empty();
            }

            int totalRecordsFiltered = totalRecords;

            return PagedList<ApplicationRole>.Create(
                query.OrderBy(sortColumnName, sortDirection),
                totalRecords,
                totalRecordsFiltered);
        }

        public async Task<IList<IdNameDto>> GetFromCacheAsync()
        {
            return await _memoryCacheService.GetSlidingExpiration(
                MemoryCacheKeys.RolesKey,
                () => 
                {
                    return _roleManager.Roles
                        .AsNoTracking()
                        .Select(r => new IdNameDto(r.Id, r.Name))
                        .ToListAsync();
                });
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole entity)
        {
            var result = await _roleManager.CreateAsync(entity);

            if (result.Succeeded)
            {
                _memoryCacheService.Remove(MemoryCacheKeys.RolesKey);
            }

            return result;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole entity)
        {
            var result = await _roleManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _memoryCacheService.Remove(MemoryCacheKeys.RolesKey);
            }

            return result;
        }

        public async Task<int> DeleteAsync(int[] ids)
        {
            ApplicationRole entity;
            IdentityResult result;
            int effectedRows = 0;

            foreach (int id in ids)
            {
                entity = await FindAsync(id);

                if (entity != null)
                {
                    result = await _roleManager.DeleteAsync(entity);

                    if (result.Succeeded)
                    {
                        effectedRows++;
                    }
                }
            }

            if (effectedRows > 0)
            {
                _memoryCacheService.Remove(MemoryCacheKeys.RolesKey);
            }

            return effectedRows;
        }
    }
}
