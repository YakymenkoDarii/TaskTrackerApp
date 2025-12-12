using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Features.Boards.Commands.CreateBoards;

public class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, int>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public CreateBoardCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<int> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var board = new Board
        {
            Title = request.Title,
            Description = request.Description,
            CreatedBy = request.CreatedBy,
            UpdatedBy = request.CreatedBy,
        };

        var newId = await uow.BoardRepository.AddAsync(board);

        await uow.SaveChangesAsync(cancellationToken);

        return newId;
    }
}