using MediatR;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Auth.Responses;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Auth.Commands.LoginCommand;

internal class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(IUnitOfWorkFactory uowFactory,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _uowFactory = uowFactory;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
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

        var accessToken = _tokenService.CreateAccessToken(user, out var accessTokenExpiration);
        var (refreshToken, refreshExpiry) = _tokenService.CreateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiration = refreshExpiry;

        await uow.UserRepository.UpdateAsync(user);
        await uow.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            Tag = user.Tag,
            Role = user.Role,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshTokenExpiration = refreshExpiry
        };
    }
}