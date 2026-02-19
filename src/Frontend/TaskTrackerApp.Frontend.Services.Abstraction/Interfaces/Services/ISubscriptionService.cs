using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface ISubscriptionService
{
    Task<Result<string?>> CreateCheckoutSessionAsync();

    Task<Result<string?>> CreatePortalSessionAsync();
}