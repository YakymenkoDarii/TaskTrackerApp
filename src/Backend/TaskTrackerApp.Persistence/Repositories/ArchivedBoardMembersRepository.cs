using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class ArchivedBoardMembersRepository : Repository<ArchivedBoardMember, int>, IArchivedBoardMembersRepository
{
    public ArchivedBoardMembersRepository(TaskTrackerDbContext context) : base(context)
    {
    }
}