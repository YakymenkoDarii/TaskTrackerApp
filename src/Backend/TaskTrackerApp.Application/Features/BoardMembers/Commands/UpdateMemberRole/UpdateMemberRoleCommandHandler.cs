using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors.Auth;
using TaskTrackerApp.Domain.Errors.Board;
using TaskTrackerApp.Domain.Errors.BoardMember;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardMembers.Commands.UpdateMemberRole;

public class UpdateMemberRoleCommandHandler : IRequestHandler<UpdateMemberRoleCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public UpdateMemberRoleCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateMemberRoleCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var currentUserId = _currentUserService.UserId;
        if (currentUserId is null)
        {
            return Result.Failure(AuthErrors.NotAuthenticated);
        }

        var board = await uow.BoardRepository.GetById(request.BoardId);

        if (board == null)
        {
            return Result.Failure(BoardErrors.NotFound);
        }

        var boardMembers = await uow.BoardMembersRepository.GetByBoardId(request.BoardId);

        var currentUserMember = boardMembers.FirstOrDefault(c => c.UserId == currentUserId);

        if (currentUserMember == null || currentUserMember.Role != BoardRole.Admin)
        {
            return Result.Failure(BoardMemberErrors.NotAuthorized);
        }

        var targetMember = boardMembers.FirstOrDefault(m => m.UserId == request.MemberId);

        if (targetMember == null)
        {
            return Result.Failure(BoardMemberErrors.NotFound);
        }

        if (targetMember.Role == BoardRole.Admin && request.Role != BoardRole.Admin)
        {
            var adminCount = boardMembers.Count(m => m.Role == BoardRole.Admin);

            if (adminCount <= 1)
            {
                return Result.Failure(BoardMemberErrors.LastAdminCannotBeDemoted);
            }
        }

        targetMember.Role = request.Role;

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}