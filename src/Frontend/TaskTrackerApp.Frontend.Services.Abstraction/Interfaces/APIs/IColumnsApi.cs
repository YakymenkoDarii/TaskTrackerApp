using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Columns;
using TaskTrackerApp.Frontend.Domain.Results;

public interface IColumnsApi
{
    [Get("/api/Columns/{boardId}")]
    Task<IApiResponse<Result<IEnumerable<ColumnDto>>>> GetByBoardIdAsync(int boardId);

    [Post("/api/Columns")]
    Task<IApiResponse<Result>> CreateAsync(CreateColumnDto columnDto);

    [Delete("/api/columns/{id}")]
    Task<IApiResponse<Result>> DeleteAsync(int id);
}