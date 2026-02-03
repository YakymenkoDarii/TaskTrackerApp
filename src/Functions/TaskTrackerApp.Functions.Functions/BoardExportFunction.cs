using MediatR;
using Microsoft.Azure.Functions.Worker;
using System.Text;
using System.Text.Json;
using TaskTrackerApp.Application.Features.Boards.Commands.MarkAsBackedUp;
using TaskTrackerApp.Application.Features.Boards.Queries.GetFullBoardDetails;
using TaskTrackerApp.Application.Interfaces.BlobStorage;
using TaskTrackerApp.Application.Interfaces.Jobs;
using TaskTrackerApp.Domain.Jobs;

namespace TaskTrackerApp.Functions.Functions;

public class BoardExportFunction
{
    private readonly ILogger<BoardExportFunction> _logger;
    private readonly IMediator _mediator;
    private readonly IBlobStorageService _blobService;
    private readonly ICosmosJobTracker _jobTracker;

    public BoardExportFunction(
        ILogger<BoardExportFunction> logger,
        IMediator mediator,
        IBlobStorageService blobService,
        ICosmosJobTracker jobTracker)
    {
        _logger = logger;
        _mediator = mediator;
        _blobService = blobService;
        _jobTracker = jobTracker;
    }

    [Function(nameof(BoardExportFunction))]
    public async Task Run(
        [ServiceBusTrigger("export-board-queue", Connection = "ServiceBusConnection")] string messageBody)
    {
        if (!int.TryParse(messageBody, out int boardId))
        {
            _logger.LogError($"Invalid Board ID received: {messageBody}");
            return;
        }

        var jobId = Guid.NewGuid().ToString();

        var job = new ArchivationJob
        {
            id = jobId,
            BoardId = boardId,
            Status = "Started",
            StartedAt = DateTime.UtcNow
        };

        var debugJson = Newtonsoft.Json.JsonConvert.SerializeObject(job);
        _logger.LogInformation("DEBUG JSON TO SAVE: {Json}", debugJson);

        await _jobTracker.CreateJobAsync(job);

        try
        {
            var query = new GetFullBoardDetailsQuery
            {
                BoardId = boardId,
            };
            var boardDto = await _mediator.Send(query);

            if (boardDto == null)
            {
                _logger.LogError($"Board {job.BoardId} was not found. Marking job as failed.");
                job.Status = "Failed";
                job.ErrorMessage = "Board not found in SQL database.";
                job.CompletedAt = DateTime.UtcNow;
                await _jobTracker.UpdateJobAsync(job);
                return;
            }

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(boardDto, jsonOptions);
            var fileName = $"board-{boardId}/{DateTime.UtcNow:yyyy-MM-dd}-backup.json";

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
            var blobUrl = await _blobService.UploadAsync(
                stream,
                containerName: "board-archives",
                blobName: fileName,
                contentType: "application/json");

            job.Status = "Completed";
            job.CompletedAt = DateTime.UtcNow;
            job.BlobUrl = blobUrl;
            await _jobTracker.UpdateJobAsync(job);

            await _mediator.Send(new MarkBoardAsBackedUpCommand(job.BoardId));

            _logger.LogInformation($"Successfully archived board {boardId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error archiving board {boardId}");

            job.Status = "Failed";
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTime.UtcNow;
            await _jobTracker.UpdateJobAsync(job);

            throw;
        }
    }
}