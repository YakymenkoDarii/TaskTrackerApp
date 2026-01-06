using MediatR;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetCardsByColumnId;

public class GetCardsByColumnIdQuery : IRequest<Result<IEnumerable<CardDto>>>
{
    public int Id { get; set; }

    public GetCardsByColumnIdQuery(int id)
    {
        Id = id;
    }
}