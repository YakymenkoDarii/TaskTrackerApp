using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken, int>, IRefreshTokenRepository
{
    public RefreshTokenRepository(TaskTrackerDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Token == token);
    }
}