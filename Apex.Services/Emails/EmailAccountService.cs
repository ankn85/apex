using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities.Emails;
using Apex.Data.Paginations;
using Apex.Data.Sorts;
using Apex.Services.Caching;
using Apex.Services.Constants;
using Microsoft.EntityFrameworkCore;

namespace Apex.Services.Emails
{
    public class EmailAccountService : BaseService<EmailAccount>, IEmailAccountService
    {
        private readonly IMemoryCacheService _memoryCacheService;

        public EmailAccountService(
            ObjectDbContext dbContext,
            IMemoryCacheService memoryCacheService)
            : base(dbContext)
        {
            _memoryCacheService = memoryCacheService;
        }

        public async Task<IPagedList<EmailAccount>> GetListAsync(
            string sortColumnName,
            SortDirection sortDirection)
        {
            var query = Table.AsNoTracking();
            int totalRecords = await query.CountAsync();

            if (totalRecords == 0)
            {
                return PagedList<EmailAccount>.Empty();
            }

            int totalRecordsFiltered = totalRecords;

            return PagedList<EmailAccount>.Create(
                query.OrderBy(sortColumnName, sortDirection),
                totalRecords,
                totalRecordsFiltered);
        }

        public async Task<EmailAccount> GetDefaultAsync()
        {
            return await _memoryCacheService.GetSlidingExpiration(
                MemoryCacheKeys.DefaultEmailAccountKey,
                () =>
                {
                    return Table.AsNoTracking()
                        .FirstOrDefaultAsync(ea => ea.IsDefaultEmailAccount);
                });
        }
    }
}
