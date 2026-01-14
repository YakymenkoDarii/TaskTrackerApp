using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface IBoardInvitationsApi
{
    [Get("/api/BoardInvitations/{boardId}")]
    Task<IApiResponse<Result<IEnumerable<BoardInvitationDto>>>> GetPendingInvitesAsync(int boardId);

    [Post("/api/BoardInvitations/invite")]
    Task<IApiResponse<Result>> SendInviteAsync(SendBoardInvitationRequestDto request);

    [Delete("/api/BoardInvitations/{invitationId}")]
    Task<IApiResponse<Result>> RevokeInviteAsync(int invitationId);

    [Get("/api/BoardInvitations/my-pending")]
    Task<IApiResponse<Result<IEnumerable<MyInvitationDto>>>> GetMyPendingInvitations();

    [Post("/api/BoardInvitations/respond")]
    Task<IApiResponse<Result>> RespondToInvite(RespondToInvitationRequestDto request);
}