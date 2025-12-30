using MediatR;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Auth;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Auth.Commands.LoginCommand;

internal class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthUserDto>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IJwtTokenService _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(IUnitOfWorkFactory uowFactory,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenGenerator)
    {
        _uowFactory = uowFactory;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthUserDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var user = request.Email is null
            ? await uow.UserRepository.GetByTagAsync(request.Tag)
            : await uow.UserRepository.GetByEmailAsync(request.Email);

        if (user is null)
            return LoginError.UserNotFound;

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return LoginError.InvalidPassword;

        return new AuthUserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role,
        };
    }
}