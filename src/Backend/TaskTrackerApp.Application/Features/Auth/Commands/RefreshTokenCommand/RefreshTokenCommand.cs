using MediatR;
using TaskTrackerApp.Domain.DTOs.Auth;

namespace TaskTrackerApp.Application.Features.Auth.Commands.RefreshTokenCommand;

public class RefreshTokenCommand : IRequest<AuthResponse>
{
    public string RefreshToken { get; set; }

    public string AccessToken { get; set; }
}