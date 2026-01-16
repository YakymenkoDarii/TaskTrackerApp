using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.CardComments.Commands.DeleteCardCommentCommand;

public class DeleteCardCommentCommand : IRequest<Result>
{
    public int Id { get; set; }
}