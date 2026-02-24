using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards.Requests;
using TaskTrackerApp.Frontend.Domain.Enums;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface IBoardsService
{
    Task<Result<IEnumerable<BoardDto>>> GetAllAsync();

    Task<Result<int>> CreateAsync(CreateBoardDto board);

    Task<Result> DeleteAsync(int boardId);

    Task<Result<BoardDto>> GetBoardByIdAsync(int boardId);

    Task<Result> UpdateAsync(int id, UpdateBoardDto dto);

    Task<Result> ArchiveBoardAsync(int boardId);

    Task<Result<IEnumerable<ArchivedBoardDto>>> GetArchivedAsync();

    Task<Result> UnArchiveBoardAsync(int boardId);

    Task<Result> TransferOwnershipAsync(int boardId, int userId);

    Task<Result<IEnumerable<BoardDto>>> GetOwnedBoardsAsync();

    Task<Result<IEnumerable<BoardDto>>> GetSharedWithMeBoardsAsync();

    Task<Result> UpdateBoardStarAsync(int boardId, UpdateStarRequest request);

    Task<Result> UpdateBoardThemeAsync(int boardId, BoardThemeColor newColor);
}