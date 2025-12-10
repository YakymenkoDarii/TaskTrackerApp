using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;
public class ColumnRepository : Repository<Column, int>, IColumnRepository
{
    public ColumnRepository(TaskTrackerDbContext context) : base(context)
    {
    }
}
