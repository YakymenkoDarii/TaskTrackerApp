using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class LabelsRepository : Repository<Label, int>, ILabelsRepository
{
    public LabelsRepository(TaskTrackerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Label>> GetLabelsByBoardIdAsync(int boardId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(l => l.BoardId == boardId)
            .ToListAsync();
    }

    public async Task DeleteWithLinksAsync(int labelId)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "DELETE FROM CardLabels WHERE LabelId = {0}", labelId);

        var label = await _dbSet.FindAsync(labelId);
        if (label != null)
        {
            _dbSet.Remove(label);
        }
    }
}