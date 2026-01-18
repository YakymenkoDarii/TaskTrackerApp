using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface ILabelsRepository : IRepository<Label, int>
{
    Task<IEnumerable<Label>> GetLabelsByBoardIdAsync(int boardId);

    Task DeleteWithLinksAsync(int labelId);
}