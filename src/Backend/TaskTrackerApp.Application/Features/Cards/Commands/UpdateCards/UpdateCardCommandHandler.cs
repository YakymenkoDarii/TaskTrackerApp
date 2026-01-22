using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.CardMappers;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Events.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Commands.UpdateCards;

internal class UpdateCardCommandHandler : IRequestHandler<UpdateCardCommand, Result<CardDto>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBoardNotifier _boardNotifier;
    private readonly IInvitationNotifier _personalNotifier;

    public UpdateCardCommandHandler(IUnitOfWorkFactory uowFactory, IBoardNotifier boardNotifier, IInvitationNotifier personalNotifier)
    {
        _uowFactory = uowFactory;
        _boardNotifier = boardNotifier;
        _personalNotifier = personalNotifier;
    }

    public async Task<Result<CardDto>> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var card = await uow.CardRepository.GetCardDetailsAsync(request.Id);
        if (card == null) return Result<CardDto>.Failure(new Error("Card.NotFound", "No card found"));
        bool isMoving = (card.ColumnId != request.ColumnId) || (card.Position != request.Position);

        int oldColumnId = card.ColumnId;
        int oldPosition = card.Position;
        int? oldAssignee = card.AssigneeId;

        if (isMoving)
        {
            var targetColumnCards = await uow.CardRepository.GetCardsByColumnIdAsync(request.ColumnId);

            int maxPos = (card.ColumnId == request.ColumnId)
                ? targetColumnCards.Count() - 1
                : targetColumnCards.Count();

            if (maxPos < 0) maxPos = 0;

            request.Position = Math.Clamp(request.Position, 0, maxPos);

            if (card.ColumnId != request.ColumnId)
            {
                var oldColumnCards = await uow.CardRepository.GetCardsByColumnIdAsync(card.ColumnId);

                foreach (var c in oldColumnCards.Where(c => c.Position > card.Position))
                {
                    c.Position--;
                    await uow.CardRepository.UpdateAsync(c);
                }

                foreach (var c in targetColumnCards.Where(c => c.Position >= request.Position))
                {
                    c.Position++;
                    await uow.CardRepository.UpdateAsync(c);
                }
            }
            else if (card.Position != request.Position)
            {
                if (request.Position < card.Position)
                {
                    var cardsToShift = targetColumnCards
                        .Where(c => c.Position >= request.Position && c.Position < card.Position && c.Id != card.Id);

                    foreach (var c in cardsToShift)
                    {
                        c.Position++;
                        await uow.CardRepository.UpdateAsync(c);
                    }
                }
                else if (request.Position > card.Position)
                {
                    var cardsToShift = targetColumnCards
                        .Where(c => c.Position > card.Position && c.Position <= request.Position && c.Id != card.Id);

                    foreach (var c in cardsToShift)
                    {
                        c.Position--;
                        await uow.CardRepository.UpdateAsync(c);
                    }
                }
            }

            card.ColumnId = request.ColumnId;
            card.Position = request.Position;
        }

        card.Title = request.Title;
        card.Description = request.Description;
        card.DueDate = request.DueDate;
        card.AssigneeId = request.AssigneeId;
        card.UpdatedById = request.UpdatedById;
        card.UpdatedAt = DateTime.UtcNow;
        card.IsCompleted = request.IsCompleted;
        card.Priority = request.Priority;

        await uow.CardRepository.UpdateAsync(card);
        await uow.SaveChangesAsync(cancellationToken);

        if (isMoving)
        {
            var moveEvt = new CardMovedEvent(card.Id, card.BoardId, card.ColumnId, card.Position);
            _ = _boardNotifier.NotifyCardMovedAsync(moveEvt);
        }
        else
        {
            var updateEvt = new CardUpdatedEvent(
                card.Id,
                card.BoardId,
                card.Title,
                card.Description,
                card.IsCompleted,
                card.DueDate,
                card.Priority,
                card.AssigneeId
            );
            _ = _boardNotifier.NotifyCardUpdatedAsync(updateEvt);
        }

        if (request.AssigneeId.HasValue && request.AssigneeId != oldAssignee)
        {
            _ = _personalNotifier.NotifyUserAssignedToCardAsync(
                request.AssigneeId.Value,
                card.Id,
                card.Title
            );
        }

        return Result<CardDto>.Success(card.ToDto());
    }
}