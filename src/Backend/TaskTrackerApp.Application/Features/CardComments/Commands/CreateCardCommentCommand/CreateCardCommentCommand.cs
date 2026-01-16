using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.CardComments.Commands.CreateCardCommentCommand;

public class CreateCardCommentCommand : IRequest<Result>
{
    public int CardId { get; set; }

    public string Text { get; set; }

    public int CreatedById { get; set; }
}