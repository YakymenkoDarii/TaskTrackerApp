using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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