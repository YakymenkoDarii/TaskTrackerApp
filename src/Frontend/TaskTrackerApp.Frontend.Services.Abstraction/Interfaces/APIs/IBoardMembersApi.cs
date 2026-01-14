using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Frontend.Domain.Enums;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface IBoardMembersApi
{
    [Get("/api/BoardMembers/{boardId}")]
    Task<IApiResponse<Result<IEnumerable<BoardMemberDto>>>> GetMembersAsync(int boardId);

    [Put("/api/BoardMembers/{boardId}/{userId}")]
    Task<IApiResponse<Result>> UpdateMemberRoleAsync(int boardId, int userId, BoardRole newRole);

    [Delete("/api/BoardMembers/{boardId}/{userId}")]
    Task<IApiResponse<Result>> DeleteMemberAsync(int boardId, int userId);
}