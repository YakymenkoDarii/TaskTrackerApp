using MediatR;
using TaskTrackerApp.Application.Features.Cards.Commands.UpdateStatusCards;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Events.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Commands.UpdateCardStatus;

public class UpdateCardStatusCommandHandler : IRequestHandler<UpdateCardStatusCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBoardNotifier _boardNotifier;

    public UpdateCardStatusCommandHandler(IUnitOfWorkFactory uowFactory, IBoardNotifier boardNotifier)
    {
        _uowFactory = uowFactory;
        _boardNotifier = boardNotifier;
    }

    public async Task<Result> Handle(UpdateCardStatusCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var card = await uow.CardRepository.GetById(request.Id);

        await uow.CardRepository.UpdateCardStatus(request.Id, request.IsCompleted);

        await uow.SaveChangesAsync();

        var evt = new CardUpdatedEvent(
            card.Id,
            card.BoardId,
            card.ColumnId,
            card.Title,
            card.AssigneeId,
            request.IsCompleted
        );

        _ = _boardNotifier.NotifyCardUpdatedAsync(evt);

        return Result.Success();
    }
}