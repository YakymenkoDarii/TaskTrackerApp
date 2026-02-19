using MediatR;

namespace TaskTrackerApp.Application.Features.Subscription.Command.RevokeSubscription;

public class RevokeSubscriptionCommand : IRequest
{
    public string StripeCustomerId { get; set; }
}