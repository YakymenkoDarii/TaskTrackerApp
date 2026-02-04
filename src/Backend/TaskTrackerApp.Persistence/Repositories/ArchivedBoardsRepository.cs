using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class ArchivedBoardsRepository : Repository<ArchivedBoard, int>, IArchivedBoardsRepository
{
    public ArchivedBoardsRepository(TaskTrackerDbContext context) : base(context)
    {
    }
}