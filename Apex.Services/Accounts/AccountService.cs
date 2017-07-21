using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities.Accounts;
using Apex.Data.Paginations;
using Microsoft.EntityFrameworkCore;

namespace Apex.Services.Accounts
{
    public class AccountService : BaseService, IAccountService
    {
        public AccountService(ObjectDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IPagedList<ApplicationUser>> GetListAsync(int page, int size)
        {
            //return await ApplicationUsers.AsNoTracking()
            //    .Include(u => u.Roles)
            //    .GetListAsync(page, size);
            throw new System.Exception();
        }

        private DbSet<ApplicationUser> ApplicationUsers
        {
            get
            {
                return _dbContext.Set<ApplicationUser>();
            }
        }
    }
}
