using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.CardComments.Commands.UpdateCardCommen;

public class UpdateCardCommentCommand : IRequest<Result>
{
    public int Id { get; set; }

    public string Text { get; set; }
}