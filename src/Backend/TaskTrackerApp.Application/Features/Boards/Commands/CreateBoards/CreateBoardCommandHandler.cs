using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.CreateBoards;

internal class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, Result<int>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public CreateBoardCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<int>> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var user = await uow.UserRepository.GetById(request.CreatedById);

        if (user != null && !user.IsPro)
        {
            var ownedBoardsCount = await uow.BoardRepository.CountByCreatorIdAsync(request.CreatedById);

            if (ownedBoardsCount >= 3)
            {
                return Result<int>.Failure(new Error(
                    "LimitReached",
                    "You have reached the free limit of 3 boards. Upgrade to Pro to create more."
                ));
            }
        }

        var board = new Board
        {
            Title = request.Title,
            Description = request.Description,
            CreatedById = request.CreatedById,
            UpdatedById = request.CreatedById,
            LastModified = DateTime.UtcNow,
            Members = new List<BoardMember>()
        };

        await uow.BoardRepository.AddAsync(board);
        await uow.SaveChangesAsync(cancellationToken);

        var adminMember = new BoardMember
        {
            BoardId = board.Id,
            UserId = request.CreatedById,
            Role = BoardRole.Admin,
            ThemeColor = request.ThemeColor,
        };

        await uow.BoardMembersRepository.AddAsync(adminMember);
        await uow.SaveChangesAsync(cancellationToken);

        return board.Id;
    }
}