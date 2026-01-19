using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class CardCommentsRepository : Repository<CardComment, int>, ICardCommentsRepository
{
    public CardCommentsRepository(TaskTrackerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CardComment>> GetByCardIdAsync(int cardId)
    {
        return await _dbSet
                .AsNoTracking()
                .Where(c => c.CardId == cardId)
                .Include(c => c.CreatedBy)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
    }
}