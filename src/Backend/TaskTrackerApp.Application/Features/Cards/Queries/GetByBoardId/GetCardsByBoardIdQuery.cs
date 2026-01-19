using MediatR;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetByBoardId;

public class GetCardsByBoardIdQuery : IRequest<Result<IEnumerable<CardDto>>>
{
    public int BoardId { get; set; }
}