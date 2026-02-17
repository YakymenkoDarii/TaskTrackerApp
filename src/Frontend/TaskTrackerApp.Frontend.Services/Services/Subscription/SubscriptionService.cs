using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Subscription;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionApi _api;

    public SubscriptionService(ISubscriptionApi api)
    {
        _api = api;
    }

    public async Task<Result<string?>> CreateCheckoutSessionAsync()
    {
        try
        {
            var response = await _api.CreateCheckoutSession();
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<string?>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<string?>.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result<string?>> CreatePortalSessionAsync()
    {
        try
        {
            var response = await _api.CreatePortalSession();
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<string?>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<string?>.Failure(new Error("UnknownError", ex.Message));
        }
    }
}