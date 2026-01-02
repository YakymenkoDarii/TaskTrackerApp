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

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}