using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface IBoardsApi
{
    [Get("/api/Boards/boards")]
    Task<IApiResponse<IEnumerable<BoardDto>>> GetAllAsync();
}