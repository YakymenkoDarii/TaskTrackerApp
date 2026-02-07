using TaskTrackerApp.Frontend.Domain.DTOs.ChatMessages;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface IChatService
{
    Task<Result<ChatResponse>> AskAsync(ChatRequest request);

    Task<Result<IEnumerable<ChatMessageDto>>> GetHistoryAsync(string sessionId);
}