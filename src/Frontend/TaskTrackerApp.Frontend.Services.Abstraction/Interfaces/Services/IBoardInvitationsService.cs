using TaskTrackerApp.Frontend.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface IBoardInvitationsService
{
    Task<Result<IEnumerable<BoardInvitationDto>>> GetPendingInvitesAsync(int boardId);

    Task<Result> SendInviteAsync(SendBoardInvitationRequestDto request);

    Task<Result> RevokeInviteAsync(int invitationId);

    Task<Result> RespondToInvite(RespondToInvitationRequestDto request);

    Task<Result<IEnumerable<MyInvitationDto>>> GetMyPendingInvitations();
}