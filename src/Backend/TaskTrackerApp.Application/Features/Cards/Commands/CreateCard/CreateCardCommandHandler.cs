using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Features.Cards.Commands.CreateCard
{
    public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCardCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                //AssigneeId = request.AssigneeId,
            };

            var newId = await _unitOfWork.CardRepository.AddAsync(card);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return newId;
        }
    }
}