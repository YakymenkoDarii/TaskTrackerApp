using TaskTrackerApp.Application.Interfaces.Repositories;

namespace TaskTrackerApp.Application.Interfaces.UoW;

public interface IUnitOfWork : IDisposable
{
    ICardRepository CardRepository { get; }

    IBoardRepository BoardRepository { get; }

    IColumnRepository ColumnRepository { get; }

    IUserRepository UserRepository { get; }

    IBoardMembersRepository BoardMembersRepository { get; }

    IBoardInvitationsRepository BoardInvitationsRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}