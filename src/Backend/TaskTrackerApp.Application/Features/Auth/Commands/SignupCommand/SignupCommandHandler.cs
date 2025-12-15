using MediatR;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Auth;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Application.Features.Auth.Commands.SignupCommand;

public class SignupCommandHandler : IRequestHandler<SignupCommand, AuthResponse>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenGenerator;

    public SignupCommandHandler(IUnitOfWorkFactory uowFactory,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenGenerator)
    {
        _uowFactory = uowFactory;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> Handle(SignupCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        if (await uow.UserRepository.GetByEmailAsync(request.Email) is not null)
        {
            return null;
        }

        if (await uow.UserRepository.GetByTagAsync(request.Tag) is not null)
        {
            return null;
        }

        var passwordHash = _passwordHasher.Generate(request.Password);

        var user = new User
        {
            Email = request.Email,
            DisplayName = request.DisplayName,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            Role = Role.User,
            Tag = request.Tag
        };

        var id = await uow.UserRepository.AddAsync(user);
        user.Id = id;

        string accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        RefreshToken refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        refreshToken.UserId = user.Id;

        await uow.RefreshTokenRepository.AddAsync(refreshToken);

        await uow.SaveChangesAsync();

        return new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken.Token };
    }
}