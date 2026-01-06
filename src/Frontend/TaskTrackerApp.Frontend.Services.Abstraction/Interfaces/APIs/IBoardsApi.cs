using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.Results;

public interface IBoardsApi
{
    [Get("/api/Boards/boards")]
    Task<IApiResponse<Result<IEnumerable<BoardDto>>>> GetAllAsync();

    [Get("/api/Boards/{boardId}")]
    Task<IApiResponse<Result<BoardDto>>> GetByIdAsync(int boardId);

    [Post("/api/Boards")]
    Task<IApiResponse<Result>> CreateAsync(CreateBoardDto boardDto);

    [Delete("/api/Boards")]
    Task<IApiResponse<Result>> DeleteAsync(int id);
}