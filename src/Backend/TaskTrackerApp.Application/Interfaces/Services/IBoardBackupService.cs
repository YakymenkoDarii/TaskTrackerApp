using TaskTrackerApp.Domain.DTOs.BackupDtos;

namespace TaskTrackerApp.Application.Interfaces.Services;

public interface IBoardBackupService
{
    Task<BoardBackupDto?> GetBackupAsync(int boardId, CancellationToken ct);

    Task DeleteBackupAsync(int boardId, CancellationToken ct);
}