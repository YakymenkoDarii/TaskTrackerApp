using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Application.Interfaces.Repositories
{
    public interface IRepository<T, TId>
    {
        Task<T?> GetAsync(TId id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<TId?> AddAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task<int> DeleteAsync(TId id);
    }
}
