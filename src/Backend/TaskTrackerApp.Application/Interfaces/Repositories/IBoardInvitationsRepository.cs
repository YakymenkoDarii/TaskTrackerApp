using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface IBoardInvitationsRepository : IRepository<BoardInvitation, int>
{
    Task<IEnumerable<BoardInvitation>> GetMyPendingAsync(int inviteeId);

    Task<BoardInvitation?> GetPendingByEmailAsync(int boardId, string email);

    Task<IEnumerable<BoardInvitation>> GetPendingInvitationsAsync(int boardId);

    Task<bool> IsInvitationPendingAsync(int boardId, string email);
}