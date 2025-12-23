using MediatR;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Auth;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Auth.Commands.LoginCommand;

internal class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
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

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var user = request.Email is null
            ? await uow.UserRepository.GetByTagAsync(request.Tag)
            : await uow.UserRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            return LoginError.UserNotFound;
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return LoginError.InvalidPassword;
        }

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        refreshToken.UserId = user.Id;

        await uow.RefreshTokenRepository.AddAsync(refreshToken);

        await uow.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }
}