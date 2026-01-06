using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Commands.UpdateCards;

internal class UpdateCardCommandHandler : IRequestHandler<UpdateCardCommand, Result<CardDto>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public UpdateCardCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<CardDto>> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var card = await uow.CardRepository.GetAsync(request.Id);

        card.ColumnId = request.ColumnId;
        card.Title = request.Title;
        card.Description = request.Description;
        card.DueDate = request.DueDate;
        card.AssigneeId = request.AssigneeId;
        card.UpdatedById = request.UpdatedById;
        card.UpdatedAt = DateTime.UtcNow;
        card.IsCompleted = request.IsCompleted;

        await uow.CardRepository.UpdateAsync(card);

        await uow.SaveChangesAsync(cancellationToken);

        return new CardDto
        {
            Id = request.Id,
            Title = card.Title,
            Description = card.Description,
            DueDate = card.DueDate,
            AssigneeId = card.AssigneeId,
            CreatedAt = card.CreatedAt,
            IsCompleted = card.IsCompleted,
            ColumnId = card.ColumnId,
        };
    }
}