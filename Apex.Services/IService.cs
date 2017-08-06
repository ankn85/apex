using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Data.Entities;

namespace Apex.Services
{
    public interface IService<T> where T : BaseEntity
    {
        Task<T> FindAsync(int id);

        Task<IList<T>> FindAsync(int[] ids);

        Task<T> CreateAsync(T entity);

        Task<int> DeleteAsync(T entity);

        Task<int> DeleteAsync(IEnumerable<T> entities);

        Task<int> CommitAsync();
    }
}
