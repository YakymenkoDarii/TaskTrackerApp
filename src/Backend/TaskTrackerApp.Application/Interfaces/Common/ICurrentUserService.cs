namespace TaskTrackerApp.Application.Interfaces.Common;

public interface ICurrentUserService
{
    int? UserId { get; }
}