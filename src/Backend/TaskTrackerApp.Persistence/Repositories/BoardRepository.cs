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

    public override async Task<Board?> GetById(int id)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<int> AddNewMemberAsync(BoardMember boardMember)
    {
        var memberEntry = await _context.BoardMembers.AddAsync(boardMember);
        return boardMember.Id;
    }

    public async Task<IEnumerable<Board>> GetAllWithOwnerAsync(int userId)
    {
        return await _dbSet
            .Where(b => b.CreatedById == userId)
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

    public async Task<bool> ArchiveBoardAsync(int boardId)
    {
        var rowsAffected = await _dbSet
            .IgnoreQueryFilters()
            .Where(b => b.Id == boardId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(board => board.IsArchived, true));

        return rowsAffected > 0;
    }

    public async Task<bool> IsBoardArchivedAsync(int boardId)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AnyAsync(b => b.Id == boardId && b.IsArchived);
    }

    public async Task<IEnumerable<int>> GetBoardIdsToArchiveAsync()
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(b => b.IsArchived && b.IsQueuedForArchival)
            .Select(b => b.Id)
            .ToListAsync();
    }

    public async Task<List<Board>> GetByCreatorIdAsync(int createdById)
    {
        return await _dbSet.Where(b => b.CreatedById == createdById).Include(b => b.Members).ToListAsync();
    }

    public async Task<int> CountByCreatorIdAsync(int createdById)
    {
        return await _dbSet.Where(b => b.CreatedById == createdById).CountAsync();
    }
}