using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Apex.Data;

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

        protected PropertyInfo GetProperty<T>(string name) where T : class
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            return properties.FirstOrDefault(p =>
                p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
