using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.DTOs.Card;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetCardById
{
    public class GetCardByIdQueryHandler : IRequestHandler<GetCardByIdQuery, CardDto>
    {
        private readonly ICardRepository _cardRepository;

        public GetCardByIdQueryHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<CardDto> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
        {
            var card = await _cardRepository.GetByIdAsync(request.Id);

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
                BoardId = card.BoardId,
                //AssigneeId = card.AssigneeId,
                CreatedAt = card.CreatedAt
            };
        }
    }
}
