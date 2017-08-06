using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apex.Data;
using Apex.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apex.Services
{
    public abstract class BaseService<T> : IService<T> where T: BaseEntity
    {
        protected readonly ObjectDbContext _dbContext;

        public BaseService(ObjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> FindAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return await Table.FindAsync(id);
        }

        public async Task<IList<T>> FindAsync(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return new List<T>();
            }

            return await Table.Where(e => ids.Contains(e.Id)).ToListAsync();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            await Table.AddAsync(entity);
            await CommitAsync();

            return entity;
        }

        public virtual async Task<int> DeleteAsync(T entity)
        {
            Table.Remove(entity);

            return await CommitAsync();
        }

        public virtual async Task<int> DeleteAsync(IEnumerable<T> entities)
        {
            Table.RemoveRange(entities);

            return await CommitAsync();
        }

        public virtual async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        protected DbSet<T> Table
        {
            get
            {
                return _dbContext.Set<T>();
            }
        }
    }
}
