using TaskTrackerApp.Functions.Functions.Data.Dtos.Board;

namespace TaskTrackerApp.Functions.Functions.Interfaces.Repositories;

public interface IBoardRepository
{
    Task DeleteAsync(int boardId);

    Task<BoardExportDto?> GetFullBoardAsync(int boardId);
}