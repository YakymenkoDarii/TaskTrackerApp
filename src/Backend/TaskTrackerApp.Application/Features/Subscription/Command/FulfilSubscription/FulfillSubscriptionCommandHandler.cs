using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Subscription.Command.FulfilSubscription;

internal class FulfillSubscriptionCommandHandler : IRequestHandler<FulfillSubscriptionCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public FulfillSubscriptionCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task Handle(FulfillSubscriptionCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var user = await uow.UserRepository.GetByEmailAsync(request.UserEmail);

        if (user == null)
        {
            return;
        }

        user.IsPro = true;
        user.SubscriptionEndDate = request.SubscriptionEndDate;
        user.StripeCustomerId = request.StripeCustomerId;

        await uow.SaveChangesAsync();
    }
}