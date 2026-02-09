using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Services;

public interface IChatHistoryService
{
    Task AddMessageAsync(string sessionId, string role, string content);

    Task<IEnumerable<ChatMessage>> GetHistoryAsync(string sessionId);
}