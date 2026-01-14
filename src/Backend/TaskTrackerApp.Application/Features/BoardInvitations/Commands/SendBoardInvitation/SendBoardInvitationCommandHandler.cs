using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Commands.SendBoardInvitation;

public class SendBoardInvitationCommandHandler : IRequestHandler<SendBoardInvitationCommand, Result<int>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public SendBoardInvitationCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<int>> Handle(SendBoardInvitationCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var existingUser = await uow.UserRepository.GetByEmailAsync(request.InviteeEmail);

        if (existingUser != null)
        {
            var isAlreadyMember = await uow.BoardMembersRepository
                .ExistsAsync(request.BoardId, existingUser.Id);

            if (isAlreadyMember)
            {
                return Result<int>.Failure(new Error("Invitation.UserIsMember", "User is already a member of this board."));
            }
        }

        var existingInvite = await uow.BoardInvitationsRepository.GetPendingByEmailAsync(request.BoardId, request.InviteeEmail);

        if (existingInvite != null)
        {
            return Result<int>.Failure(new Error("Invitation.AlreadyPending", "An invitation is already pending for this user."));
        }

        var invitation = new BoardInvitation
        {
            BoardId = request.BoardId,
            SenderId = request.SenderId,
            InviteeEmail = request.InviteeEmail,
            InviteeId = existingUser?.Id,
            Role = request.Role,
            Status = InvitationStatus.Pending,
        };

        await uow.BoardInvitationsRepository.AddAsync(invitation);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(invitation.Id);
    }
}