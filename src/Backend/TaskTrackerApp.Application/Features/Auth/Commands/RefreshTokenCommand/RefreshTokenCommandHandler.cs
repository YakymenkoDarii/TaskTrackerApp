using MediatR;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Auth.Responses;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Auth.Commands.RefreshTokenCommand;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(IUnitOfWorkFactory uowFactory, ITokenService tokenService)
    {
        _uowFactory = uowFactory;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var user = await uow.UserRepository.GetByRefreshTokenAsync(request.RefreshToken);
        if (user == null)
        {
            return LoginError.UserNotFound;
        }

        if (user.RefreshToken != request.RefreshToken)
        {
            return LoginError.InvalidRefreshToken;
        }

        if (user.RefreshTokenExpiration < DateTime.UtcNow)
        {
            return LoginError.RefreshTokenExpired;
        }

        var accessToken = _tokenService.CreateAccessToken(user, out var accessTokenExpiration);
        var (newRefreshToken, newRefreshExpiry) = _tokenService.CreateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiration = newRefreshExpiry;

        await uow.UserRepository.UpdateAsync(user);
        await uow.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            Tag = user.Tag,
            Role = user.Role,
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshTokenExpiration = newRefreshExpiry
        };
    }
}