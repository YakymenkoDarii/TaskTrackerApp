using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetAllMyAssignedCards;

public class GetAllMyAssignedCardsQuery : IRequest<Result<IEnumerable<UpcomingCardDto>>>
{
}