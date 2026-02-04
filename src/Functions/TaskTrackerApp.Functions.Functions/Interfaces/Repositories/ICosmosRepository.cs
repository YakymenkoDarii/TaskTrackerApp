using TaskTrackerApp.Functions.Functions.Data.Cosmos;

namespace TaskTrackerApp.Functions.Functions.Interfaces.Repositories;

public interface ICosmosRepository
{
    Task CreateBackupAsync(BoardBackupDocument backup);

    Task<BoardBackupDocument?> GetBackupByBoardIdAsync(int boardId);

    Task UpsertJobAsync(ArchivationJob job);
}