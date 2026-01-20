using MediatR;
using TaskTrackerApp.Application.Features.Cards.Commands.DeleteCard;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Events.Card;

namespace TaskTrackerApp.Application.Features.Cards.Commands.DeleteCards;

internal class DeleteCardCommandHandler : IRequestHandler<DeleteCardCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBoardNotifier _boardNotifier;

    public DeleteCardCommandHandler(IUnitOfWorkFactory uowFactory, IBoardNotifier boardNotifier)
    {
        _uowFactory = uowFactory;
        _boardNotifier = boardNotifier;
    }

    public async Task Handle(DeleteCardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var card = await uow.CardRepository.GetById(request.Id);
        int boardId = card.BoardId;

        await uow.CardRepository.DeleteAsync(request.Id);
        await uow.SaveChangesAsync(cancellationToken);

        var evt = new CardDeletedEvent(request.Id, boardId);
        _ = _boardNotifier.NotifyCardDeletedAsync(evt);
    }
}