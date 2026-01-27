using MediatR;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Users.Commands.ChangePassword;

internal class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(IUnitOfWorkFactory uowFactory, IPasswordHasher passwordHasher, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.NewPasswordConfirm)
        {
            return Result.Failure(UserErrors.PasswordMismatch);
        }

        var userId = _currentUserService.UserId;
        using var uow = _uowFactory.Create();

        var user = await uow.UserRepository.GetById(userId.Value);
        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        bool isOldPasswordCorrect = _passwordHasher.Verify(request.OldPassword, user.PasswordHash);

        if (!isOldPasswordCorrect)
        {
            return Result.Failure(UserErrors.InvalidPassword);
        }

        string newHash = _passwordHasher.Generate(request.NewPassword);

        user.PasswordHash = newHash;

        await uow.UserRepository.UpdateAsync(user);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}