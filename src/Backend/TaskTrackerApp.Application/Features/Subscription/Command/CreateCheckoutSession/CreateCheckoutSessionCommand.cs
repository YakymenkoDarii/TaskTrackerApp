using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Subscription.Command.CreateCheckoutSession;

public class CreateCheckoutSessionCommand : IRequest<Result<string>>
{
}