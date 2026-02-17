using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Services;

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(User user);

    Task<string> CreatePortalSessionAsync(string stripeCustomerId);
}