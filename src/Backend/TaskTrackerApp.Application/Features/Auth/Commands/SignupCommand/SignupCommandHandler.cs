using MediatR;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Auth;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Auth.Commands.SignupCommand;

internal class SignupCommandHandler : IRequestHandler<SignupCommand, Result<AuthResponse>>
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

    public async Task<Result<AuthResponse>> Handle(SignupCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        if (await uow.UserRepository.GetByEmailAsync(request.Email) is not null)
        {
            return SignupError.EmailInUse;
        }
        if (await uow.UserRepository.GetByTagAsync(request.Tag) is not null)
        {
            return SignupError.TagInUse;
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

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }
}