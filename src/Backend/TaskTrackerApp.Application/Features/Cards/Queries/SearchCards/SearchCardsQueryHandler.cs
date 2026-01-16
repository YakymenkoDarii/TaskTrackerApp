using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.CardMappers;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Queries.SearchCards;

public class SearchCardsQueryHandler : IRequestHandler<SearchCardsQuery, Result<IEnumerable<CardDto>>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public SearchCardsQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<CardDto>>> Handle(SearchCardsQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var query = uow.CardRepository.GetQueryable();

        if (request.BoardId.HasValue)
        {
            query = query.Where(c => c.Column.BoardId == request.BoardId.Value);
        }
        else if (request.AssigneeId.HasValue)
        {
            query = query.Where(c => c.AssigneeId == request.AssigneeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c => c.Title.Contains(request.SearchTerm));
        }

        var result = query
            .Take(10)
            .Select(c => c.ToDto())
            .ToList();

        return Result<IEnumerable<CardDto>>.Success(result);
    }
}