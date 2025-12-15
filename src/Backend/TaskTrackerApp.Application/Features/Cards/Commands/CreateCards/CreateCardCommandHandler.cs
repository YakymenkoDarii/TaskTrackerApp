using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Features.Cards.Commands.CreateCard;

public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, int>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public CreateCardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<int> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.Create();

        var card = new Card
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            ColumnId = request.ColumnId,
            BoardId = request.BoardId,
            AssigneeId = request.AssigneeId,
            CreatedById = request.CreatedById,
            UpdatedById = request.CreatedById
        };

        var newId = await uow.CardRepository.AddAsync(card);

        await uow.SaveChangesAsync(cancellationToken);

        return newId;
    }
}