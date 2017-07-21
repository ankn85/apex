using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public async Task<EmailAccount> GetAsync(int id)
        {
            return await EmailAccounts.FindAsync(id);
        }

        public async Task<IPagedList<EmailAccountDto>> GetListAsync(
            string sortColumnName,
            SortDirection sortDirection)
        {
            int totalRecords = await EmailAccounts.CountAsync();
            int page = 0, size = 0;

            if (totalRecords == 0)
            {
                return new PagedList<EmailAccountDto>(page, size);
            }

            int totalRecordsFiltered = totalRecords;

            return new PagedList<EmailAccountDto>(
                GetPagedEmailAccounts(EmailAccounts.AsNoTracking(), sortColumnName, sortDirection),
                totalRecords,
                totalRecordsFiltered,
                page,
                size);
        }

        public async Task<EmailAccount> GetDefaultAsync()
        {
            return await _memoryCacheService.GetSlidingExpiration(
                MemoryCacheKeys.DefaultEmailAccountKey,
                () =>
                {
                    return EmailAccounts.AsNoTracking()
                        .FirstOrDefaultAsync(ea => ea.IsDefaultEmailAccount);
                });
        }

        public async Task<EmailAccount> CreateAsync(EmailAccount entity)
        {
            await EmailAccounts.AddAsync(entity);
            await CommitAsync();

            return entity;
        }

        public async Task<int> UpdateAsync(EmailAccount entity)
        {
            EmailAccount updatedEntity = await EmailAccounts.FindAsync(entity.Id);

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
            EmailAccount entity = await EmailAccounts.FindAsync(id);

            if (entity == null)
            {
                throw new ApiException(
                    $"{nameof(EmailAccount)} not found. Id = {id}",
                    ApiErrorCode.NotFound);
            }

            EmailAccounts.Remove(entity);

            return await CommitAsync();
        }

        private DbSet<EmailAccount> EmailAccounts
        {
            get
            {
                return _dbContext.Set<EmailAccount>();
            }
        }

        private IEnumerable<EmailAccountDto> GetPagedEmailAccounts(
            IQueryable<EmailAccount> emailAccounts,
            string sortColumnName,
            SortDirection sortDirection)
        {
            PropertyInfo property = GetProperty<EmailAccount>(sortColumnName);

            var sortEmailAccounts = sortDirection == SortDirection.Ascending ?
                emailAccounts.OrderBy(property.GetValue) :
                emailAccounts.OrderByDescending(property.GetValue);

            return sortEmailAccounts
                .Select(ea => new EmailAccountDto(ea.Id, ea.Email, ea.DisplayName, ea.Host, ea.Port, ea.EnableSsl, ea.UseDefaultCredentials, ea.IsDefaultEmailAccount));
        }
    }
}
