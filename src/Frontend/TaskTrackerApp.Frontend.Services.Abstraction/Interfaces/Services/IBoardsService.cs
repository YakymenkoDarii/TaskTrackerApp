using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface IBoardsService
{
    Task<Result<IEnumerable<BoardDto>>> GetAllAsync();

    Task<Result> CreateAsync(CreateBoardDto board);

    Task<Result> DeleteAsync(int boardId);
}