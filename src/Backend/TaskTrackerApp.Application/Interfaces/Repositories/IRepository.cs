namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface IRepository<T, TId>
{
    Task<T?> GetById(TId id);

    Task<IEnumerable<T>> GetAllAsync();

    Task<TId?> AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(TId id);
}