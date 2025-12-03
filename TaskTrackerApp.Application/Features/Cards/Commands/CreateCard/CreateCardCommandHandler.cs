using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Features.Cards.Commands.CreateCard
{
    public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, int>
    {
        private readonly ICardRepository _cardRepository;

        public CreateCardCommandHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<int> Handle(CreateCardCommand request, CancellationToken cancellationToken)
        {
            var card = new Card
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                ColumnId = request.ColumnId,
                BoardId = request.BoardId,
                AssigneeId = request.AssigneeId,
            };

            var createdCard = await _cardRepository.AddAsync(card);
            return createdCard.Id;
        }
    }
}
