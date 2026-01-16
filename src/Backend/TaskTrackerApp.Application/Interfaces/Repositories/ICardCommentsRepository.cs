using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface ICardCommentsRepository : IRepository<CardComment, int>
{
    Task<IEnumerable<CardComment>> GetByCardIdAsync(int cardId);
}