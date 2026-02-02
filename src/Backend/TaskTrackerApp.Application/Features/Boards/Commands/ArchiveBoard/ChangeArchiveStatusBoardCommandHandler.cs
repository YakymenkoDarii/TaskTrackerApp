using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors.Board;
using TaskTrackerApp.Domain.Errors.BoardMember;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.ArchiveBoard;

public class ChangeArchiveStatusBoardCommandHandler : IRequestHandler<ChangeArchiveStatusBoardCommand, Result>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWorkFactory _uowFactory;

    public ChangeArchiveStatusBoardCommandHandler(ICurrentUserService currentUserService, IUnitOfWorkFactory uowFactory)
    {
        _currentUserService = currentUserService;
        _uowFactory = uowFactory;
    }

    public async Task<Result> Handle(ChangeArchiveStatusBoardCommand request, CancellationToken cancellationToken)
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

        var isChanged = await uow.BoardRepository.ChangeBoardArchiveStatus(request.BoardId);

        if (!isChanged)
        {
            return BoardErrors.NotFound;
        }
        await uow.SaveChangesAsync();

        return Result.Success();
    }
}