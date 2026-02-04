namespace TaskTrackerApp.Application.Interfaces.Jobs;

public interface IArchivalSyncJob
{
    Task RunAsync();
}