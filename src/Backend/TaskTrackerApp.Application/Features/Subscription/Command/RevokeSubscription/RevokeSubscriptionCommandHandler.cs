using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Subscription.Command.RevokeSubscription;

internal class RevokeSubscriptionCommandHandler : IRequestHandler<RevokeSubscriptionCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public RevokeSubscriptionCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task Handle(RevokeSubscriptionCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var user = await uow.UserRepository.GetByStripeIdAsync(request.StripeCustomerId);

        if (user == null)
        {
            return;
        }

        user.IsPro = false;
        user.SubscriptionEndDate = null;

        await uow.SaveChangesAsync();
    }
}