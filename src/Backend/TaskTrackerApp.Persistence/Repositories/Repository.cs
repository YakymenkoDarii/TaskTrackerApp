using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class
{
    protected readonly TaskTrackerDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(TaskTrackerDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetById(TId id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<TId?> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);

        return (TId?)_context.Entry(entity).Property("Id").CurrentValue;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _context.Entry(entity).State = EntityState.Modified;
    }

    public async Task DeleteAsync(TId id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }
}