using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors.Board;
using TaskTrackerApp.Domain.Errors.BoardMember;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.ArchiveBoard;

public class ArchiveBoardCommandHandler : IRequestHandler<ArchiveBoardCommand, Result>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWorkFactory _uowFactory;

    public ArchiveBoardCommandHandler(
        ICurrentUserService currentUserService,
        IUnitOfWorkFactory uowFactory)
    {
        _currentUserService = currentUserService;
        _uowFactory = uowFactory;
    }

    public async Task<Result> Handle(ArchiveBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        if (_currentUserService.UserId is not { } userId)
        {
            return BoardMemberErrors.NotAuthorized;
        }

        var boardMember = await uow.BoardMembersRepository
            .GetMemberAsync(request.BoardId, userId);

        if (boardMember == null)
        {
            return BoardMemberErrors.NotFound;
        }

        if (boardMember.Role != BoardRole.Admin)
        {
            return BoardMemberErrors.NotAuthorized;
        }

        var boardMembers = await uow.BoardMembersRepository.GetByBoardId(request.BoardId);
        var board = await uow.BoardRepository.GetById(request.BoardId);

        if (board == null)
        {
            return BoardErrors.NotFound;
        }

        board.IsArchived = true;
        board.IsQueuedForArchival = true;

        await uow.BoardRepository.UpdateAsync(board);

        var newArchivedBoard = new ArchivedBoard
        {
            Title = board.Title,
            Description = board.Description,
            OriginalBoardId = board.Id
        };

        await uow.ArchivedBoardsRepository.AddAsync(newArchivedBoard);

        foreach (var member in boardMembers)
        {
            var archMember = new ArchivedBoardMember
            {
                UserId = member.UserId,
                Role = member.Role,
                ArchivedBoard = newArchivedBoard
            };

            await uow.ArchivedBoardMembersRepository.AddAsync(archMember);
        }

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}