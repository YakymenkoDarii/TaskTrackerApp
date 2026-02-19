using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Subscription.Command.CreateCustomerPortal;

public class CreateCustomerPortalCommand : IRequest<Result<string>>
{
}