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
        return await _dbSet
                    .Include(m => m.Board)
                    .Where(c => c.UserId == userId)
                    .Where(c => !c.Board.IsArchived)
                    .ToListAsync();
    }

    public async Task<IEnumerable<BoardMember>> GetMembershipsWithBoardDetailsAsync(int userId)
    {
        return await _context.BoardMembers
            .Where(bm => bm.UserId == userId)
            .Include(bm => bm.Board)
                .ThenInclude(b => b.Members)
                    .ThenInclude(m => m.User)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int boardId, int userId)
    {
        return await _dbSet
            .AnyAsync(m => m.BoardId == boardId && m.UserId == userId);
    }

    public async Task<BoardMember> GetMemberAsync(int boardId, int userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(bm => bm.UserId == userId && bm.BoardId == boardId);
    }

    public async Task<IEnumerable<BoardMember>> GetArchivedByUserIdAsync(int userId)
    {
        return await _dbSet
                .Include(m => m.Board)
                .IgnoreQueryFilters()
                .Where(c => c.UserId == userId)
                .Where(c => c.Board.IsArchived)
                .ToListAsync();
    }
}