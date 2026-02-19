using MediatR;

namespace TaskTrackerApp.Application.Features.Subscription.Command.FulfilSubscription;

public class FulfillSubscriptionCommand : IRequest
{
    public string UserEmail { get; set; }

    public string StripeCustomerId { get; set; }

    public DateTime SubscriptionEndDate { get; set; }
}