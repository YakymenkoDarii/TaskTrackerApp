using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class ColumnRepository : Repository<Column, int>, IColumnRepository
{
    public ColumnRepository(TaskTrackerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Column>> GetColumnsByBoardIdAsync(int boardId)
    {
        return await _dbSet
            .Where(c => c.BoardId == boardId)
            .ToListAsync();
    }
}