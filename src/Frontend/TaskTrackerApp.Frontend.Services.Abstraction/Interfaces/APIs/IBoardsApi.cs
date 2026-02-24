using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards.Requests;
using TaskTrackerApp.Frontend.Domain.Results;

public interface IBoardsApi
{
    [Get("/api/Boards/boards")]
    Task<IApiResponse<Result<IEnumerable<BoardDto>>>> GetAllAsync();

    [Get("/api/Boards/{boardId}")]
    Task<IApiResponse<Result<BoardDto>>> GetByIdAsync(int boardId);

    [Post("/api/Boards")]
    Task<IApiResponse<Result<int>>> CreateAsync(CreateBoardDto boardDto);

    [Delete("/api/Boards")]
    Task<IApiResponse<Result>> DeleteAsync(int id);

    [Put("/api/Boards/{id}")]
    Task<IApiResponse<Result>> UpdateAsync(int id, UpdateBoardDto boardDto);

    [Put("/api/Boards/arhcive/{boardId}")]
    Task<IApiResponse<Result>> ArchiveBoardAsync(int boardId);

    [Get("/api/Boards/archived")]
    Task<IApiResponse<Result<IEnumerable<ArchivedBoardDto>>>> GetArchivedAsync();

    [Put("/api/Boards/unarchive/{boardId}")]
    Task<IApiResponse<Result>> UnArchiveBoardAsync(int boardId);

    [Put("/api/Boards/transferOwnership/{boardId}/{userId}")]
    Task<IApiResponse<Result>> TransferOwnership(int boardId, int userId);

    [Get("/api/Boards/owned")]
    Task<IApiResponse<Result<IEnumerable<BoardDto>>>> GetOwnedBoards();

    [Get("/api/Boards/sharedWithMe")]
    Task<IApiResponse<Result<IEnumerable<BoardDto>>>> GetSharedWithMeBoards();

    [Put("/api/Boards/{boardId}/star")]
    Task<IApiResponse<Result>> UpdateBoardStar(int boardId, [Body] UpdateStarRequest request);
}