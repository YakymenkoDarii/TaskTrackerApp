using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface ICardRepository : IRepository<Card, int>
{
    Task<IEnumerable<Card>> GetCardsByBoardIdAsync(int boardId);

    Task<IEnumerable<Card>> GetCardsByColumnIdAsync(int columnId);

    Task UpdateCardStatus(int id, bool isComplete);

    Task<IEnumerable<Card>> GetUpcomingCardsAsync(int userId, DateTime start, DateTime end, bool includeOverdue);

    Task<IEnumerable<Card>> GetCardsByAsigneeIdAsync(int userId);

    IQueryable<Card> GetQueryable();
}