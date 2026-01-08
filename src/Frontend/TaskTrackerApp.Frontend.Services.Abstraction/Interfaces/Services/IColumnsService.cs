using TaskTrackerApp.Frontend.Domain.DTOs.Columns;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface IColumnsService
{
    Task<Result<IEnumerable<ColumnDto>>> GetByBoardIdAsync(int boardId);

    Task<Result<int>> CreateColumnAsync(CreateColumnDto columnDto);

    Task<Result> DeleteColumnAsync(int id);

    Task<Result> UpdateColumnAsync(int id, UpdateColumnDto columnDto);
}