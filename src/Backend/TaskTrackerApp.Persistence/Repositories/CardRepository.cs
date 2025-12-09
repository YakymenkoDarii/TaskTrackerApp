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
}