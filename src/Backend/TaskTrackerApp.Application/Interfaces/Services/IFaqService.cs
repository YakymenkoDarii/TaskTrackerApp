namespace TaskTrackerApp.Application.Interfaces.Services;

public interface IFaqService
{
    Task<string> AskQuestionAsync(string question);

    Task IngestDummyDataAsync();
}