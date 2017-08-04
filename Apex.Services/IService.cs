using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Data.Entities;

namespace Apex.Services
{
    public interface IService
    {
        Task<T> FindAsync<T>(int id) where T : BaseEntity;

        Task<IList<T>> FindAsync<T>(int[] ids) where T : BaseEntity;

        Task<T> CreateAsync<T>(T entity) where T : BaseEntity;

        Task<int> DeleteAsync<T>(T entity) where T : BaseEntity;

        Task<int> DeleteAsync<T>(IEnumerable<T> entities) where T : BaseEntity;

        Task<int> CommitAsync();
    }
}
