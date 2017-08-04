using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities;
using Apex.Data.Sorts;
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

        #region CRUD

        public virtual async Task<T> FindAsync<T>(int id) where T : BaseEntity
        {
            if (id <= 0)
            {
                return null;
            }

            return await Table<T>().FindAsync(id);
        }

        public async Task<IList<T>> FindAsync<T>(int[] ids) where T : BaseEntity
        {
            if (ids == null || ids.Length == 0)
            {
                return new List<T>();
            }

            return await Table<T>().Where(e => ids.Contains(e.Id)).ToListAsync();
        }

        public virtual async Task<T> CreateAsync<T>(T entity) where T : BaseEntity
        {
            await Table<T>().AddAsync(entity);
            await CommitAsync();

            return entity;
        }

        public virtual async Task<int> DeleteAsync<T>(T entity) where T : BaseEntity
        {
            Table<T>().Remove(entity);

            return await CommitAsync();
        }

        public virtual async Task<int> DeleteAsync<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            Table<T>().RemoveRange(entities);

            return await CommitAsync();
        }

        public virtual async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        protected DbSet<T> Table<T>() where T : BaseEntity
        {
            return _dbContext.Set<T>();
        }

        #endregion

        protected IEnumerable<T> GetSortList<T>(
            IQueryable<T> source,
            string sortColumnName,
            SortDirection sortDirection) where T : BaseEntity
        {
            return sortDirection == SortDirection.Ascending ?
                source.OrderBy(sortColumnName) :
                source.OrderByDescending(sortColumnName);
        }

        protected IEnumerable<T> GetSortAndPagedList<T>(
            IQueryable<T> source,
            string sortColumnName,
            SortDirection sortDirection,
            int page,
            int size) where T : BaseEntity
        {
            return GetSortList(source, sortColumnName, sortDirection)
                .Skip(page)
                .Take(size);
        }
    }
}
