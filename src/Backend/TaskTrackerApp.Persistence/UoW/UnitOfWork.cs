using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Persistence.Contexts;
using TaskTrackerApp.Persistence.Repositories;

namespace TaskTrackerApp.Persistence.UoW;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly TaskTrackerDbContext _dbContext;

    private ICardRepository? _cardRepository;
    private IBoardRepository? _boardRepository;
    private IColumnRepository? _columnRepository;
    private IUserRepository? _userRepository;
    private IBoardMembersRepository _boardMembersRepository;
    private IBoardInvitationsRepository _boardInvintationsRepository;
    private ICardCommentsRepository? _cardCommentsRepository;
    private ILabelsRepository? _labelsRepository;
    private ICommentAttachmentsRepository? _commentAttachmentsRepository;

    public UnitOfWork(TaskTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ICardRepository CardRepository =>
        _cardRepository ??= new CardRepository(_dbContext);

    public IBoardRepository BoardRepository =>
        _boardRepository ??= new BoardRepository(_dbContext);

    public IColumnRepository ColumnRepository =>
        _columnRepository ??= new ColumnRepository(_dbContext);

    public IUserRepository UserRepository =>
    _userRepository ??= new UserRepository(_dbContext);

    public IBoardMembersRepository BoardMembersRepository =>
        _boardMembersRepository ??= new BoardMembersRepository(_dbContext);

    public IBoardInvitationsRepository BoardInvitationsRepository =>
        _boardInvintationsRepository ??= new BoardInvitationsRepository(_dbContext);

    public ICardCommentsRepository CardCommentsRepository =>
        _cardCommentsRepository ??= new CardCommentsRepository(_dbContext);

    public ILabelsRepository LabelsRepository =>
        _labelsRepository ??= new LabelsRepository(_dbContext);

    public ICommentAttachmentsRepository CommentAttachmentsRepository =>
        _commentAttachmentsRepository ??= new CommentAttachmentsRepository(_dbContext);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}