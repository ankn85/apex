using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities.Accounts;

namespace Apex.Services.Accounts
{
    public class ACLService : BaseService<ApplicationRoleMenu>, IACLService
    {
        private readonly IRoleService _roleService;

        public ACLService(
            ObjectDbContext dbContext,
            IRoleService roleService) : base(dbContext)
        {
            _roleService = roleService;
        }

        public async Task GetListAsync()
        {
            var roles = await _roleService.GetFromCacheAsync();
        }
    }
}
