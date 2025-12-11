using MediatR;

namespace TaskTrackerApp.Application.Features.Cards.Commands.DeleteCard;

public class DeleteCardCommand : IRequest
{
    public int Id { get; set; }
}