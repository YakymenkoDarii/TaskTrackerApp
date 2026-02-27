using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.CardMappers;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetAllMyAssignedCards;

internal class GetAllMyAssignedCardsQueryHandler : IRequestHandler<GetAllMyAssignedCardsQuery, Result<IEnumerable<UpcomingCardDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetAllMyAssignedCardsQueryHandler(ICurrentUserService currentUserService, IUnitOfWorkFactory uowFactory)
    {
        _currentUserService = currentUserService;
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<UpcomingCardDto>>> Handle(GetAllMyAssignedCardsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return UserErrors.Unauthorized;

        using var uow = _uowFactory.Create();

        var cards = await uow.CardRepository.GetCardsByAsigneeIdAsync(userId.Value);

        var cardDtos = new List<UpcomingCardDto>();

        foreach (var card in cards)
        {
            if (card.IsCompleted)
            {
                continue;
            }

            cardDtos.Add(CardMappers.ToUpcomingDto(card));
        }

        return cardDtos;
    }
}