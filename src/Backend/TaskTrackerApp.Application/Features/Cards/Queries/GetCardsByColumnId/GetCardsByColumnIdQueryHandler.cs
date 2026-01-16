using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.CardMappers;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetCardsByColumnId;

public class GetCardsByColumnIdQueryHandler : IRequestHandler<GetCardsByColumnIdQuery, Result<IEnumerable<CardDto>>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetCardsByColumnIdQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<CardDto>>> Handle(GetCardsByColumnIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var cards = await uow.CardRepository.GetCardsByColumnIdAsync(request.Id);

        if (cards == null)
        {
            return null;
        }

        var cardDtos = cards.Select(x => x!.ToDto())
            .OrderBy(c => c.Position)
            .ToList();

        return cardDtos;
    }
}