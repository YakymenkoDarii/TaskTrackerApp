using TaskTrackerApp.Functions.Functions.Data.Cosmos;
using TaskTrackerApp.Functions.Functions.Interfaces.Repositories;
using TaskTrackerApp.Functions.Functions.Interfaces.Services;

namespace TaskTrackerApp.Functions.Functions.Services;

public class BoardArchivalService : IBoardArchivalService
{
    private readonly IBlobStorageService _blobService;
    private readonly IBoardRepository _boardRepository;
    private readonly ICosmosRepository _cosmosRepository;

    public BoardArchivalService(
        IBlobStorageService blobService,
        IBoardRepository boardRepository,
        ICosmosRepository cosmosRepository)
    {
        _blobService = blobService;
        _boardRepository = boardRepository;
        _cosmosRepository = cosmosRepository;
    }

    public async Task ArchiveBoard(int boardId)
    {
        var job = new ArchivationJob
        {
            Id = boardId.ToString(),
            BoardId = boardId,
            Status = "Started",
            StartedAt = DateTime.UtcNow
        };
        await _cosmosRepository.UpsertJobAsync(job);

        try
        {
            var boardData = await _boardRepository.GetFullBoardAsync(boardId);
            if (boardData == null)
            {
                job.Status = "Failed";
                job.FailureReason = "Board not found in SQL";
                job.CompletedAt = DateTime.UtcNow;
                await _cosmosRepository.UpsertJobAsync(job);
                return;
            }

            var blobPath = $"board-{boardId}/data.json";
            var blobUrl = await _blobService.UploadBackupAsync(boardData, blobPath);

            job.Status = "Uploaded";
            await _cosmosRepository.UpsertJobAsync(job);

            var existingBackup = await _cosmosRepository.GetBackupByBoardIdAsync(boardId);
            if (existingBackup == null)
            {
                await _cosmosRepository.CreateBackupAsync(new BoardBackupDocument
                {
                    BoardId = boardData.Id,
                    Title = boardData.Title,
                    BlobUrl = blobUrl,
                    ArchivedBy = boardData.CreatedById.ToString()
                });
            }

            await _boardRepository.DeleteAsync(boardId);

            job.Status = "Completed";
            job.CompletedAt = DateTime.UtcNow;
            await _cosmosRepository.UpsertJobAsync(job);
        }
        catch (Exception ex)
        {
            job.Status = "Failed";
            job.FailureReason = ex.Message;
            job.CompletedAt = DateTime.UtcNow;

            await _cosmosRepository.UpsertJobAsync(job);

            throw;
        }
    }
}