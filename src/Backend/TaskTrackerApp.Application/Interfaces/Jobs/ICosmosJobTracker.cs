using TaskTrackerApp.Domain.Jobs;

namespace TaskTrackerApp.Application.Interfaces.Jobs;

public interface ICosmosJobTracker
{
    Task CreateJobAsync(ArchivationJob job);
    Task DeleteJobByBoardIdAsync(int boardId);
    Task<ArchivationJob?> GetJobByBoardIdAsync(int boardId);
    Task UpdateJobAsync(ArchivationJob job);
}