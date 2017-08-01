using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Data.Entities.Accounts;
using Apex.Services.Caching;
using Apex.Services.Constants;
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

        public async Task<IList<ApplicationRole>> GetListAsync()
        {
            return await _memoryCacheService.GetSlidingExpiration(
                MemoryCacheKeys.RolesKey,
                () =>
                {
                    return _roleManager.Roles.ToListAsync();
                });
        }
    }
}
