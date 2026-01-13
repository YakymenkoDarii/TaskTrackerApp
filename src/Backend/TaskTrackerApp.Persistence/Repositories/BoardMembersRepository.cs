using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class BoardMembersRepository : Repository<BoardMember, int>, IBoardMembersRepository
{
    public BoardMembersRepository(TaskTrackerDbContext context) : base(context)
    {
    }

    public async Task<bool> ChangeRoleAsync(int boardId, int userId, BoardRole role)
    {
        var rowsAffected = await _dbSet
                .Where(c => c.BoardId == boardId && c.UserId == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.Role, role));

        return rowsAffected == 1;
    }

    public async Task<IEnumerable<BoardMember>> GetByBoardId(int boardId)
    {
        var members = await _dbSet
                .Include(m => m.User)
                .Where(c => c.BoardId == boardId)
                .ToListAsync();

        return members;
    }

    public async Task<IEnumerable<BoardMember>> GetByUserIdAsync(int userId)
    {
        var members = await _dbSet
                .Include(m => m.Board)
                .Where(c => c.UserId == userId)
                .ToListAsync();

        return members;
    }

    public async Task<bool> ExistsAsync(int boardId, int userId)
    {
        return await _dbSet
            .AnyAsync(m => m.BoardId == boardId && m.UserId == userId);
    }
}