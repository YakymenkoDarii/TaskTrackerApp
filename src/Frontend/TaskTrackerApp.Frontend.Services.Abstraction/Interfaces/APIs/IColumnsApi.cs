using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.DTOs.Columns;
using TaskTrackerApp.Frontend.Domain.Results;

public interface IColumnsApi
{
    [Get("/api/Columns/{boardId}")]
    Task<IApiResponse<Result<IEnumerable<ColumnDto>>>> GetByBoardIdAsync(int boardId);

    [Post("/api/Columns")]
    Task<IApiResponse<Result<int>>> CreateAsync(CreateColumnDto columnDto);

    [Delete("/api/Columns/{id}")]
    Task<IApiResponse<Result>> DeleteAsync(int id);

    [Put("/api/Columns/{id}")]
    Task<IApiResponse<Result<CardDto>>> UpdateAsync(int id, UpdateColumnDto columnDto);
}