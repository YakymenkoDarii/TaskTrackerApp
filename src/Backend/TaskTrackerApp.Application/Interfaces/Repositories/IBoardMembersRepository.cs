using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface IBoardMembersRepository : IRepository<BoardMember, int>
{
    Task<bool> ChangeRoleAsync(int boardId, int userId, BoardRole role);

    Task<bool> ExistsAsync(int boardId, int userId);

    Task<IEnumerable<BoardMember>> GetArchivedByUserIdAsync(int userId);

    Task<IEnumerable<BoardMember>> GetByBoardId(int boardId);

    Task<IEnumerable<BoardMember>> GetByUserIdAsync(int userId);

    Task<BoardMember> GetMemberAsync(int boardId, int userId);
    Task<IEnumerable<BoardMember>> GetMembershipsWithBoardDetailsAsync(int userId);
}