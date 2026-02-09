using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;
using TaskTrackerApp.Frontend.Domain.DTOs.ChatMessages;

namespace TaskTrackerApp.Application.Features.Faq.Queries.GetHistory;

public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, Result<IEnumerable<ChatMessageDto>>>
{
    private readonly IChatHistoryService _historyService;

    public GetChatHistoryQueryHandler(IChatHistoryService historyService)
    {
        _historyService = historyService;
    }

    public async Task<Result<IEnumerable<ChatMessageDto>>> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var historyEntities = await _historyService.GetHistoryAsync(request.SessionId);

            var historyDtos = historyEntities.Select(msg => new ChatMessageDto
            {
                Role = msg.Role,
                Content = msg.Content,
            });

            return Result<IEnumerable<ChatMessageDto>>.Success(historyDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ChatMessageDto>>.Failure(new Error("HistoryError", $"Failed to retrieve chat history: {ex.Message}"));
        }
    }
}