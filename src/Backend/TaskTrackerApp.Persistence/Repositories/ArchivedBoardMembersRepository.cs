using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class ArchivedBoardMembersRepository : Repository<ArchivedBoardMember, int>, IArchivedBoardMembersRepository
{
    public ArchivedBoardMembersRepository(TaskTrackerDbContext context) : base(context)
    {
    }

    public async Task<ArchivedBoardMember?> GetMemberAsync(int archivedBoardId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.ArchivedBoardMembers
            .FirstOrDefaultAsync(m => m.ArchivedBoardId == archivedBoardId && m.UserId == userId, cancellationToken);
    }

    public async Task<List<ArchivedBoardMember>> GetMembersByBoardIdAsync(int archivedBoardId, CancellationToken cancellationToken = default)
    {
        return await _context.ArchivedBoardMembers
            .Where(m => m.ArchivedBoardId == archivedBoardId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ArchivedBoardMember>> GetByUserIdAsync(int userId)
    {
        return _dbSet
            .Where(bm => bm.UserId == userId)
            .AsNoTracking();
    }
}