using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;
public class BoardRepository : Repository<Board, int>, IBoardRepository
{
    public BoardRepository(TaskTrackerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Board>> GetAllWithOwnerAsync(int userId)
    {
        return await _dbSet
            .Where(b => b.CreatedBy == userId)
            .ToListAsync();

    }

    public async Task<IEnumerable<User>> GetMembersAsync(int boardId)
    {
        return await _context.Users
            .Where(u => _context.BoardMembers
                .Where(bm => bm.BoardId == boardId)
                .Select(bm => bm.UserId)
                .Contains(u.Id))
            .ToListAsync();
    }
}
