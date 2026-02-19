using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors.Auth;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Subscription.Command.CreateCheckoutSession;

internal class CreateCheckoutSessionCommandHandler : IRequestHandler<CreateCheckoutSessionCommand, Result<string>>
{
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public CreateCheckoutSessionCommandHandler(
        IPaymentService paymentService,
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService)
    {
        _paymentService = paymentService;
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<string>> Handle(CreateCheckoutSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return AuthErrors.NotAuthenticated;
        }

        using var uow = _uowFactory.Create();

        var user = await uow.UserRepository.GetById(userId.Value);

        var url = await _paymentService.CreateCheckoutSessionAsync(user);

        return url;
    }
}