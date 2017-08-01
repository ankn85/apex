using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities;
using Apex.Services.Enums;
using Microsoft.EntityFrameworkCore;

namespace Apex.Services
{
    public abstract class BaseService : IService
    {
        protected readonly ObjectDbContext _dbContext;

        public BaseService(ObjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        protected DbSet<T> Table<T>() where T : BaseEntity
        {
            return _dbContext.Set<T>();
        }

        protected IEnumerable<T> GetSortList<T>(
            IQueryable<T> source,
            string sortColumnName,
            SortDirection sortDirection) where T : class
        {
            PropertyInfo property = GetProperty<T>(sortColumnName);

            if (property == null)
            {
                return source;
            }

            return sortDirection == SortDirection.Ascending ?
                source.OrderBy(property.GetValue) :
                source.OrderByDescending(property.GetValue);
        }

        protected IEnumerable<T> GetSortAndPagedList<T>(
            IQueryable<T> source,
            string sortColumnName,
            SortDirection sortDirection,
            int page,
            int size) where T : class
        {
            return GetSortList(source, sortColumnName, sortDirection)
                .Skip(page)
                .Take(size);
        }

        private PropertyInfo GetProperty<T>(string name) where T : class
        {
            return typeof(T).GetProperties()
                .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
