using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Commands.UpdateStatusCards;

public class UpdateCardStatusCommand : IRequest<Result>
{
    public int Id { get; set; }

    public bool IsCompleted { get; set; }
}