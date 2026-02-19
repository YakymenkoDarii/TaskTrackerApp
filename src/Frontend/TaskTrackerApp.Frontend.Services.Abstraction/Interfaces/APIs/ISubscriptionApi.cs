using Refit;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface ISubscriptionApi
{
    [Post("/api/Subscription/checkout")]
    Task<IApiResponse<Result<string?>>> CreateCheckoutSession();

    [Post("/api/Subscription/portal")]
    Task<IApiResponse<Result<string?>>> CreatePortalSession();
}