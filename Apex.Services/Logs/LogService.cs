using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities.Logs;
using Apex.Data.Paginations;
using Apex.Services.Enums;
using Apex.Services.Extensions;
using Apex.Services.Models;
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
            return await Logs.FindAsync(id);
        }

        public async Task<IPagedList<LogDto>> GetListAsync(
            DateTime fromDate,
            DateTime toDate,
            string level,
            string sortColumnName,
            SortDirection sortDirection,
            int page,
            int size)
        {
            int totalRecords = await Logs.CountAsync();

            if (totalRecords == 0)
            {
                return new PagedList<LogDto>(page, size);
            }

            var filterLogs = GetFilterLogs(fromDate, toDate, level);
            int totalRecordsFiltered = await filterLogs.CountAsync();

            if (totalRecordsFiltered == 0)
            {
                return new PagedList<LogDto>(totalRecords, page, size);
            }

            return new PagedList<LogDto>(
                GetSortAndPagedLogs(filterLogs, sortColumnName, sortDirection, page, size),
                totalRecords,
                totalRecordsFiltered,
                page,
                size);
        }

        public async Task<int> DeleteAsync(int[] ids)
        {
            var dbSet = Logs;
            var logs = dbSet.Where(l => ids.Contains(l.Id));

            if (!logs.Any())
            {
                throw new ApiException(
                    $"Logs not found. Ids = {string.Join(",", ids)}",
                    ApiErrorCode.NotFound);
            }

            dbSet.RemoveRange(logs);

            return await CommitAsync();
        }

        private DbSet<Log> Logs
        {
            get
            {
                return _dbContext.Set<Log>();
            }
        }

        private IQueryable<Log> GetFilterLogs(DateTime fromDate, DateTime toDate, string level)
        {
            DateTime startDate = fromDate.StartOfDay();
            DateTime endDate = toDate.EndOfDay();

            var logs = Logs.AsNoTracking()
                .Where(l => startDate <= l.Logged && l.Logged <= endDate);

            if (!string.IsNullOrEmpty(level))
            {
                logs = logs.Where(l => l.Level.Equals(level, StringComparison.OrdinalIgnoreCase));
            }

            return logs;
        }

        private IEnumerable<LogDto> GetSortAndPagedLogs(
            IQueryable<Log> logs,
            string sortColumnName,
            SortDirection sortDirection,
            int page,
            int size)
        {
            PropertyInfo property = GetProperty<Log>(sortColumnName);

            var sortLogs = sortDirection == SortDirection.Ascending ?
                logs.OrderBy(property.GetValue) :
                logs.OrderByDescending(property.GetValue);

            return sortLogs
                .Skip(page)
                .Take(size)
                .Select(l => new LogDto(l.Id, l.Application, l.Logged, l.Level, l.Message, l.Logger, l.Callsite, l.Exception));
        }
    }
}
