using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface IBoardRepository : IRepository<Board, int>
{
    Task<IEnumerable<Board>> GetAllWithOwnerAsync(int userId);

    Task<IEnumerable<User>> GetMembersAsync(int boardId);

    Task<int> AddNewMemberAsync(BoardMember boardMember);

    Task<bool> ArchiveBoardAsync(int boardId);

    Task<bool> IsBoardArchivedAsync(int boardId);

    Task<IEnumerable<int>> GetBoardIdsToArchiveAsync();

    Task<IEnumerable<Board>> GetByCreatorIdAsync(int createdById);

    Task<int> CountByCreatorIdAsync(int createdById);
}