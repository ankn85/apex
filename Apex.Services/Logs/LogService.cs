using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities.Logs;
using Apex.Data.Paginations;
using Apex.Services.Enums;
using Apex.Services.Extensions;
using Apex.Services.Models;
using Apex.Services.Models.Logs;
using Microsoft.EntityFrameworkCore;

namespace Apex.Services.Logs
{
    public class LogService : BaseService, ILogService
    {
        public LogService(ObjectDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<Log> FindAsync(int id)
        {
            return await Table<Log>().FindAsync(id);
        }

        public async Task<IPagedList<LogDto>> GetListAsync(
            DateTime fromDate,
            DateTime toDate,
            IList<string> levels,
            string sortColumnName,
            SortDirection sortDirection,
            int page,
            int size)
        {
            var query = Table<Log>().AsNoTracking();
            int totalRecords = await query.CountAsync();

            if (totalRecords == 0)
            {
                return PagedList<LogDto>.Empty();
            }

            query = GetFilterList(query, fromDate, toDate, levels);
            int totalRecordsFiltered = await query.CountAsync();

            if (totalRecordsFiltered == 0)
            {
                return PagedList<LogDto>.Empty(totalRecords);
            }

            return PagedList<LogDto>.Create(
                GetSortAndPagedList(query, sortColumnName, sortDirection, page, size).Select(l => ToLogDto(l)),
                totalRecords,
                totalRecordsFiltered);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var dbSet = Table<Log>();
            Log entity = await dbSet.FindAsync(id);

            if (entity == null)
            {
                throw new ApiException(
                    $"{nameof(Log)} not found. Id = {id}",
                    ApiErrorCode.NotFound);
            }

            dbSet.Remove(entity);

            return await CommitAsync();
        }

        public async Task<int> DeleteAsync(int[] ids)
        {
            var dbSet = Table<Log>();
            var entities = dbSet.Where(l => ids.Contains(l.Id));

            if (!entities.Any())
            {
                throw new ApiException(
                    $"Logs not found. Ids = {string.Join(",", ids)}",
                    ApiErrorCode.NotFound);
            }

            dbSet.RemoveRange(entities);

            return await CommitAsync();
        }

        private IQueryable<Log> GetFilterList(
            IQueryable<Log> source,
            DateTime fromDate,
            DateTime toDate,
            IList<string> levels)
        {
            DateTime startDate = fromDate.StartOfDay();
            DateTime endDate = toDate.EndOfDay();

            source = source.Where(l => startDate <= l.Logged && l.Logged <= endDate);

            if (levels != null && levels.Count > 0)
            {
                source = source.Where(l => levels.Contains(l.Level));
            }

            return source;
        }

        private LogDto ToLogDto(Log entity)
        {
            return new LogDto(
                entity.Id,
                entity.Application,
                entity.Logged,
                entity.Level,
                entity.Message,
                entity.Logger,
                entity.Callsite,
                entity.Exception);
        }
    }
}
