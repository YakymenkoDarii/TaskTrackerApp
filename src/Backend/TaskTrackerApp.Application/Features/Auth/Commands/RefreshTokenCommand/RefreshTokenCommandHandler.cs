using MediatR;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Domain.DTOs.Auth;

namespace TaskTrackerApp.Application.Features.Auth.Commands.RefreshTokenCommand;

internal class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse?>
{
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(IJwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    public Task<AuthResponse?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return _jwtTokenService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);
    }
}