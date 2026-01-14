using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class CardRepository : Repository<Card, int>, ICardRepository
{
    public CardRepository(TaskTrackerDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Card>> GetCardsByBoardIdAsync(int boardId)
    {
        return await _dbSet
            .Where(c => c.BoardId == boardId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Card>> GetCardsByColumnIdAsync(int columnId)
    {
        return await _dbSet
            .Where(c => c.ColumnId == columnId)
            .ToListAsync();
    }

    public async Task UpdateCardStatus(int id, bool isComplete)
    {
        await _dbSet.Where(c => c.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.IsCompleted, isComplete));
    }

    public async Task<IEnumerable<Card>> GetUpcomingCardsAsync(int userId, DateTime start, DateTime end, bool includeOverdue)
    {
        var query = _dbSet.AsQueryable();

        query = query.Where(c => c.AssigneeId == userId && c.DueDate.HasValue);

        if (includeOverdue)
        {
            query = query.Where(c =>
                (c.DueDate >= start && c.DueDate <= end) ||
                (c.DueDate < start && !c.IsCompleted));
        }
        else
        {
            query = query.Where(c => c.DueDate >= start && c.DueDate <= end);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Card>> GetCardsByAsigneeIdAsync(int userId)
    {
        var query = _dbSet.AsQueryable();

        query = query.Where(c => c.AssigneeId == userId);

        return await query.ToListAsync();
    }

    public IQueryable<Card> GetQueryable()
    {
        return _dbSet
            .Include(c => c.Column)
            .AsNoTracking()
            .AsQueryable();
    }
}