using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Events.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Commands.CreateCard;

internal class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, Result<int>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IBoardNotifier _boardNotifier;
    private readonly IInvitationNotifier _personalNotifier;

    public CreateCardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier, IInvitationNotifier personalNotifier)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _boardNotifier = boardNotifier;
        _personalNotifier = personalNotifier;
    }

    public async Task<Result<int>> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.Create();

        var cards = await uow.CardRepository.GetCardsByBoardIdAsync(request.BoardId);
        int newLastPosition;

        if (cards.Any())
        {
            newLastPosition = cards.Max(c => c.Position) + 1;
        }
        else
        {
            newLastPosition = 0;
        }

        var card = new Card
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            ColumnId = request.ColumnId,
            BoardId = request.BoardId,
            AssigneeId = request.AssigneeId,
            CreatedById = request.CreatedById,
            UpdatedById = request.CreatedById,
            Position = newLastPosition,
        };

        await uow.CardRepository.AddAsync(card);

        await uow.SaveChangesAsync(cancellationToken);

        var boardEvt = new CardCreatedEvent(
            card.Id,
            card.BoardId,
            card.ColumnId,
            card.Title,
            card.Description,
            card.AssigneeId,
            card.Priority.ToString()
        );

        _ = _boardNotifier.NotifyCardCreatedAsync(boardEvt);

        if (card.AssigneeId.HasValue)
        {
            _ = _personalNotifier.NotifyUserAssignedToCardAsync(
                card.AssigneeId.Value,
                card.Id,
                card.Title
            );
        }

        return card.Id;
    }
}