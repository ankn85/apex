using System.Linq;
using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities.Emails;
using Apex.Data.Paginations;
using Apex.Services.Caching;
using Apex.Services.Constants;
using Apex.Services.Enums;
using Apex.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Apex.Services.Emails
{
    public class EmailAccountService : BaseService, IEmailAccountService
    {
        private readonly IMemoryCacheService _memoryCacheService;

        public EmailAccountService(
            ObjectDbContext dbContext,
            IMemoryCacheService memoryCacheService)
            : base(dbContext)
        {
            _memoryCacheService = memoryCacheService;
        }

        public async Task<EmailAccount> FindAsync(int id)
        {
            return await Table<EmailAccount>().FindAsync(id);
        }

        public async Task<IPagedList<EmailAccount>> GetListAsync(
            string sortColumnName,
            SortDirection sortDirection)
        {
            var query = Table<EmailAccount>().AsNoTracking();
            int totalRecords = await query.CountAsync();

            if (totalRecords == 0)
            {
                return PagedList<EmailAccount>.Empty();
            }

            int totalRecordsFiltered = totalRecords;

            return PagedList<EmailAccount>.Create(
                GetSortList(query, sortColumnName, sortDirection),
                totalRecords,
                totalRecordsFiltered);
        }

        public async Task<EmailAccount> GetDefaultAsync()
        {
            return await _memoryCacheService.GetSlidingExpiration(
                MemoryCacheKeys.DefaultEmailAccountKey,
                () =>
                {
                    return Table<EmailAccount>().AsNoTracking()
                        .FirstOrDefaultAsync(ea => ea.IsDefaultEmailAccount);
                });
        }

        public async Task<EmailAccount> CreateAsync(EmailAccount entity)
        {
            await Table<EmailAccount>().AddAsync(entity);
            await CommitAsync();

            return entity;
        }

        public async Task<int> UpdateAsync(EmailAccount entity)
        {
            EmailAccount updatedEntity = await Table<EmailAccount>().FindAsync(entity.Id);

            if (updatedEntity == null)
            {
                throw new ApiException(
                    $"{nameof(EmailAccount)} not found. Id = {entity.Id}",
                    ApiErrorCode.NotFound);
            }

            updatedEntity.Email = entity.Email;
            updatedEntity.DisplayName = entity.DisplayName;
            updatedEntity.Host = entity.Host;
            updatedEntity.Port = entity.Port;
            updatedEntity.UserName = entity.UserName;
            updatedEntity.Password = entity.Password;
            updatedEntity.EnableSsl = entity.EnableSsl;
            updatedEntity.UseDefaultCredentials = entity.UseDefaultCredentials;
            updatedEntity.IsDefaultEmailAccount = entity.IsDefaultEmailAccount;

            return await CommitAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var dbSet = Table<EmailAccount>();
            EmailAccount entity = await dbSet.FindAsync(id);

            if (entity == null)
            {
                throw new ApiException(
                    $"{nameof(EmailAccount)} not found. Id = {id}",
                    ApiErrorCode.NotFound);
            }

            dbSet.Remove(entity);

            return await CommitAsync();
        }

        public async Task<int> DeleteAsync(int[] ids)
        {
            var dbSet = Table<EmailAccount>();
            var entities = dbSet.Where(l => ids.Contains(l.Id));

            if (!entities.Any())
            {
                throw new ApiException(
                    $"EmailAccounts not found. Ids = {string.Join(",", ids)}",
                    ApiErrorCode.NotFound);
            }

            dbSet.RemoveRange(entities);

            return await CommitAsync();
        }
    }
}
