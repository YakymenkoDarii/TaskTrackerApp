using MediatR;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetUpcomingCardsByDate;

public class GetUpcomingCardsByDateQuery : IRequest<Result<IEnumerable<UpcomingCardDto>>>
{
    public int UserId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IncludeOverdue { get; set; }
}