using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class UserRepository : Repository<User, int>, IUserRepository
{
    public UserRepository(TaskTrackerDbContext context) : base(context)
    {
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User> GetByTagAsync(string tag)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Tag == tag);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
            .SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}