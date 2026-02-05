using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface IArchivedBoardMembersRepository : IRepository<ArchivedBoardMember, int>
{
    Task<IEnumerable<ArchivedBoardMember>> GetByUserIdAsync(int userId);
    Task<ArchivedBoardMember?> GetMemberAsync(int archivedBoardId, int userId, CancellationToken cancellationToken = default);
    Task<List<ArchivedBoardMember>> GetMembersByBoardIdAsync(int archivedBoardId, CancellationToken cancellationToken = default);
}