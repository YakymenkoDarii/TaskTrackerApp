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

    public async Task<IEnumerable<User>> SearchAsync(string term, int? excludeBoardId = null)
    {
        var query = _dbSet.AsQueryable();

        string lowerTerm = term.ToLower();

        query = query.Where(u => u.DisplayName.ToLower().Contains(lowerTerm) ||
                                 u.Email.ToLower().Contains(lowerTerm) ||
                                 u.Tag.ToLower().Contains(lowerTerm));

        if (excludeBoardId.HasValue)
        {
            query = query.Where(u => !u.BoardMemberships.Any(m => m.BoardId == excludeBoardId.Value));
        }

        return await query
            .Take(10)
            .ToListAsync();
    }
}