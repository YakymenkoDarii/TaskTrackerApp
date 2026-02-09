using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.ChatMessages;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface IChatApi
{
    [Post("/api/Faq/ask")]
    Task<IApiResponse<Result<ChatResponse>>> AskAsync([Body] ChatRequest request);

    [Get("/api/Faq/history/{sessionId}")]
    Task<IApiResponse<Result<IEnumerable<ChatMessageDto>>>> GetHistoryAsync(string sessionId);
}