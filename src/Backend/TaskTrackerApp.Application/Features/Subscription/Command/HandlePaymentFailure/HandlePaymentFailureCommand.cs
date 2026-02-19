using MediatR;

namespace TaskTrackerApp.Application.Features.Subscription.Command.HandlePaymentFailure;

public class HandlePaymentFailureCommand : IRequest
{
    public string StripeCustomerId { get; set; }

    public string FailureReason { get; set; }
}