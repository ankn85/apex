using System;
using System.Linq;
using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities.Emails;
using Apex.Data.Paginations;
using Apex.Services.Enums;
using Apex.Services.Extensions;
using Apex.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Apex.Services.Emails
{
    public class QueuedEmailService : BaseService<QueuedEmail>, IQueuedEmailService
    {
        public QueuedEmailService(ObjectDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IPagedList<QueuedEmail>> GetListAsync(
            DateTime? createdFromUtc,
            DateTime? createdToUtc,
            bool loadNotSentItemsOnly,
            bool loadOnlyItemsToBeSent,
            int maxSendTries,
            bool loadNewest,
            int page,
            int size)
        {
            var query = Table
                .Include(qe => qe.EmailAccount)
                .AsNoTracking();

            int totalRecords = await query.CountAsync();

            if (totalRecords == 0)
            {
                return PagedList<QueuedEmail>.Empty();
            }

            query = GetFilterList(
                query,
                createdFromUtc,
                createdToUtc,
                loadNotSentItemsOnly,
                loadOnlyItemsToBeSent,
                maxSendTries,
                loadNewest);
            int totalRecordsFiltered = await query.CountAsync();

            if (totalRecordsFiltered == 0)
            {
                return PagedList<QueuedEmail>.Empty(totalRecords);
            }

            var pagedList = query.Skip(page).Take(size);

            return PagedList<QueuedEmail>.Create(pagedList, totalRecords, totalRecordsFiltered);
        }

        public async Task<QueuedEmail> CreateAsync(string email, string subject, string message, EmailAccount emailAccount)
        {
            QueuedEmail queuedEmail = new QueuedEmail
            {
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = email,
                Subject = subject,
                Body = message,
                CreatedOnUtc = DateTime.UtcNow,
                Priority = (int)QueuedEmailPriority.High,
                EmailAccountId = emailAccount.Id
            };

            return await CreateAsync(queuedEmail);
        }

        public async Task<int> UpdateAsync(int id, int sentTries, DateTime? sentOnUtc, string failedReason)
        {
            QueuedEmail updatedEntity = await FindAsync(id);

            if (updatedEntity == null)
            {
                throw new ApiException($"{nameof(QueuedEmailService)} » {nameof(UpdateAsync)}] Queued Email not found. Id = {id}");
            }

            updatedEntity.SentTries = sentTries;
            updatedEntity.SentOnUtc = sentOnUtc;
            updatedEntity.FailedReason = failedReason;

            return await CommitAsync();
        }

        private IQueryable<QueuedEmail> GetFilterList(
            IQueryable<QueuedEmail> source,
            DateTime? createdFromUtc,
            DateTime? createdToUtc,
            bool loadNotSentItemsOnly,
            bool loadOnlyItemsToBeSent,
            int maxSendTries,
            bool loadNewest)
        {
            if (createdFromUtc.HasValue)
            {
                var startDate = createdFromUtc.Value.StartOfDay();
                source = source.Where(qe => qe.CreatedOnUtc >= startDate);
            }

            if (createdToUtc.HasValue)
            {
                var endDate = createdToUtc.Value.EndOfDay();
                source = source.Where(qe => qe.CreatedOnUtc <= endDate);
            }

            if (loadNotSentItemsOnly)
            {
                source = source.Where(qe => !qe.SentOnUtc.HasValue);
            }

            if (loadOnlyItemsToBeSent)
            {
                DateTime nowUtc = DateTime.UtcNow;
                source = source.Where(qe => !qe.DontSendBeforeDateUtc.HasValue || qe.DontSendBeforeDateUtc.Value <= nowUtc);
            }

            source = source.Where(qe => qe.SentTries < maxSendTries);

            source = loadNewest ?
                source.OrderByDescending(qe => qe.CreatedOnUtc) :
                source.OrderByDescending(qe => qe.Priority).ThenBy(qe => qe.CreatedOnUtc);

            return source;
        }

        private DbSet<EmailAccount> EmailAccounts
        {
            get
            {
                return _dbContext.Set<EmailAccount>();
            }
        }
    }
}
