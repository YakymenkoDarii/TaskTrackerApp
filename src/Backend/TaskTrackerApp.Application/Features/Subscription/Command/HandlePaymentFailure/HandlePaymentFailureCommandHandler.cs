using MediatR;
using Microsoft.Extensions.Logging;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Subscription.Command.HandlePaymentFailure;

internal class HandlePaymentFailureCommandHandler : IRequestHandler<HandlePaymentFailureCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ILogger<HandlePaymentFailureCommandHandler> _logger;

    public HandlePaymentFailureCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ILogger<HandlePaymentFailureCommandHandler> logger)
    {
        _uowFactory = uowFactory;
        _logger = logger;
    }

    public async Task Handle(HandlePaymentFailureCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var user = await uow.UserRepository.GetByStripeIdAsync(request.StripeCustomerId);

        if (user == null)
        {
            _logger.LogError($"Payment failed for unknown Stripe Customer: {request.StripeCustomerId}");
            return;
        }
        _logger.LogWarning($"PAYMENT FAILED: User {user.Email} (ID: {user.Id}) could not be charged. Reason: {request.FailureReason}");

        //Later this can be used to send email
        //Like "There wasn't enough money on the card"
    }
}