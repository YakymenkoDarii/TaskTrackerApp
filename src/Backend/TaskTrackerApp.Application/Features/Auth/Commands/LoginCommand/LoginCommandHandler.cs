using MediatR;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Auth;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Features.Auth.Commands.LoginCommand;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
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

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        User user;

        if (request.Email is not null)
        {
            user = await uow.UserRepository.GetByEmailAsync(request.Email);
        }
        else
        {
            user = await uow.UserRepository.GetByTagAsync(request.Tag);
        }

        if (user is null)
        {
            return null;
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return null;
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