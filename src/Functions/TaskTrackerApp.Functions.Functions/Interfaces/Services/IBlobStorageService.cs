namespace TaskTrackerApp.Functions.Functions.Interfaces.Services;

public interface IBlobStorageService
{
    Task<string> UploadBackupAsync<T>(T data, string fileName);
}