using Microsoft.Azure.Functions.Worker;
using TaskTrackerApp.Functions.Functions.Interfaces.Services;

namespace TaskTrackerApp.Functions.Functions;

public class BoardExportFunction
{
    private readonly ILogger<BoardExportFunction> _logger;
    private readonly IBoardArchivalService _archivalService;

    public BoardExportFunction(
        ILogger<BoardExportFunction> logger,
        IBoardArchivalService archivalService)
    {
        _logger = logger;
        _archivalService = archivalService;
    }

    [Function(nameof(BoardExportFunction))]
    public async Task Run(
        [ServiceBusTrigger("export-board-queue", Connection = "ServiceBusConnection")] string messageBody)
    {
        _logger.LogInformation($"Function triggered for message: {messageBody}");

        if (!int.TryParse(messageBody, out int boardId))
        {
            _logger.LogError($"Invalid Board ID received: {messageBody}");
            return;
        }

        try
        {
            _logger.LogInformation($"[Board {boardId}] Starting archival process...");

            await _archivalService.ArchiveBoard(boardId);

            _logger.LogInformation($"[Board {boardId}] Successfully archived and removed from SQL.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[Board {boardId}] CRITICAL FAILURE during archival.");
            throw;
        }
    }
}