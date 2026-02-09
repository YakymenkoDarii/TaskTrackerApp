using MediatR;
using TaskTrackerApp.Domain.Results;
using TaskTrackerApp.Frontend.Domain.DTOs.ChatMessages;

namespace TaskTrackerApp.Application.Features.Faq.Commands.AskQuestion;

public class AskQuestionCommand : IRequest<Result<ChatResponse>>
{
    public string SessionId { get; set; }

    public string Question { get; set; }
}