using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Application.Features.Boards.Commands.CreateBoards;

internal class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, int>
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
            CreatedById = request.CreatedById,
            UpdatedById = request.CreatedById,
            Members = new List<BoardMember>()
        };

        await uow.BoardRepository.AddAsync(board);
        await uow.SaveChangesAsync(cancellationToken);

        var adminMember = new BoardMember
        {
            BoardId = board.Id,
            UserId = request.CreatedById,
            Role = BoardRole.Admin
        };

        await uow.BoardMembersRepository.AddAsync(adminMember);
        await uow.SaveChangesAsync(cancellationToken);

        return board.Id;
    }
}