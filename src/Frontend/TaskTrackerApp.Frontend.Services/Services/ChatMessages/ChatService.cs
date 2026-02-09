using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.ChatMessages;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.ChatMessages;

public class ChatService : IChatService
{
    private readonly IChatApi _chatApi;

    public ChatService(IChatApi chatApi)
    {
        _chatApi = chatApi;
    }

    public async Task<Result<ChatResponse>> AskAsync(ChatRequest request)
    {
        try
        {
            var response = await _chatApi.AskAsync(request);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<ChatResponse>.Failure(new Error("NetworkError", ex.Message));
        }
        catch (Exception ex)
        {
            return Result<ChatResponse>.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result<IEnumerable<ChatMessageDto>>> GetHistoryAsync(string sessionId)
    {
        try
        {
            var response = await _chatApi.GetHistoryAsync(sessionId);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<IEnumerable<ChatMessageDto>>.Failure(new Error("NetworkError", ex.Message));
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ChatMessageDto>>.Failure(new Error("UnknownError", ex.Message));
        }
    }
}