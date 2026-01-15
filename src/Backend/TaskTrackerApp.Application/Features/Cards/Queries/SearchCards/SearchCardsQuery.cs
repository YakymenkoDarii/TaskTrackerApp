using MediatR;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Queries.SearchCards;

public class SearchCardsQuery : IRequest<Result<IEnumerable<CardDto>>>
{
    public string SearchTerm { get; set; }

    public int? BoardId { get; set; }

    public int? AssigneeId { get; set; }
}