using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface IColumnRepository : IRepository<Column, int>
{
    Task<IEnumerable<Column>> GetColumnsByBoardIdAsync(int boardId);
}