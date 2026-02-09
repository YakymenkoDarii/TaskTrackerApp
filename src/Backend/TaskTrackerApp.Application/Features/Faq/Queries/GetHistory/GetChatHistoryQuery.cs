using MediatR;
using TaskTrackerApp.Domain.Results;
using TaskTrackerApp.Frontend.Domain.DTOs.ChatMessages;

namespace TaskTrackerApp.Application.Features.Faq.Queries.GetHistory;

public class GetChatHistoryQuery : IRequest<Result<IEnumerable<ChatMessageDto>>>
{
    public string SessionId { get; set; }
}