using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface ICardRepository : IRepository<Card, int>
{
    Task<IEnumerable<Card>> GetCardsByBoardIdAsync(int boardId);

    Task<IEnumerable<Card>> GetCardsByColumnIdAsync(int columnId);
}