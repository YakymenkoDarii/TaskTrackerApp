using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors.Auth;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Subscription.Command.CreateCustomerPortal;

internal class CreateCustomerPortalCommandHandler : IRequestHandler<CreateCustomerPortalCommand, Result<string>>
{
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public CreateCustomerPortalCommandHandler(
        IPaymentService paymentService,
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService)
    {
        _paymentService = paymentService;
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<string>> Handle(CreateCustomerPortalCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        using var uow = _uowFactory.Create();

        if (userId == null)
        {
            return AuthErrors.NotAuthenticated;
        }

        var userStripeId = await uow.UserRepository.GetStripeIdByUserIdAsync(userId.Value);

        if (userStripeId == null)
        {
            return UserErrors.NotFound;
        }

        var url = await _paymentService.CreatePortalSessionAsync(userStripeId);

        return url;
    }
}