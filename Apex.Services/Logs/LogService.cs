using System;
using System.Linq;
using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities.Logs;
using Apex.Data.Paginations;
using Apex.Data.Sorts;
using Apex.Services.Extensions;
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

        public async Task<IPagedList<LogDto>> GetListAsync(
            DateTime fromDate,
            DateTime toDate,
            string[] levels,
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
                GetSortAndPagedList(query, sortColumnName, sortDirection, page, size).Select(l => new LogDto(l)),
                totalRecords,
                totalRecordsFiltered);
        }

        private IQueryable<Log> GetFilterList(
            IQueryable<Log> source,
            DateTime fromDate,
            DateTime toDate,
            string[] levels)
        {
            DateTime startDate = fromDate.StartOfDay();
            DateTime endDate = toDate.EndOfDay();

            source = source.Where(l => startDate <= l.Logged && l.Logged <= endDate);

            if (levels != null && levels.Length > 0)
            {
                source = source.Where(l => levels.Contains(l.Level));
            }

            return source;
        }
    }
}
