using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetUpcomingCardsByDate;

public class GetUpcomingCardsByDateQueryHandler : IRequestHandler<GetUpcomingCardsByDateQuery, Result<IEnumerable<UpcomingCardDto>>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetUpcomingCardsByDateQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<UpcomingCardDto>>> Handle(GetUpcomingCardsByDateQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var cards = await uow.CardRepository.GetUpcomingCardsAsync(
            request.UserId,
            request.StartDate,
            request.EndDate,
            request.IncludeOverdue
        );

        var dtos = cards.Select(c => new UpcomingCardDto
        {
            Id = c.Id,
            Title = c.Title,
            DueDate = c.DueDate,
            IsCompleted = c.IsCompleted,
            BoardId = c.BoardId,
            BoardTitle = c.Column?.Board?.Title,
        });

        return dtos.ToList();
    }
}