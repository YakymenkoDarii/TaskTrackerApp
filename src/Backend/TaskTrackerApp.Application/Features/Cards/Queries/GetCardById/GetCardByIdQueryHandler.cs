using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Card;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetCardById
{
    public class GetCardByIdQueryHandler : IRequestHandler<GetCardByIdQuery, CardDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCardByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CardDto> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
        {
            var card = await _unitOfWork.CardRepository.GetAsync(request.Id);

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