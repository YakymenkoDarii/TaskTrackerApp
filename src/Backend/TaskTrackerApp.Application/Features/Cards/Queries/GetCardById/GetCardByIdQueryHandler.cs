using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Card;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetCardById;

public class GetCardByIdQueryHandler : IRequestHandler<GetCardByIdQuery, CardDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public GetCardByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<CardDto> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.Create();

        var card = await uow.CardRepository.GetAsync(request.Id);

        if (card == null)
        {
            return null;
        }

        return new CardDto
        {
            Id = card.Id,
            Title = card.Title,
            Description = card.Description,
            DueDate = card.DueDate,
            ColumnId = card.ColumnId,
            AssigneeId = card.AssigneeId,
            CreatedAt = card.CreatedAt,
            IsCompleted = card.IsCompleted,
            Position = card.Position,
        };
    }
}