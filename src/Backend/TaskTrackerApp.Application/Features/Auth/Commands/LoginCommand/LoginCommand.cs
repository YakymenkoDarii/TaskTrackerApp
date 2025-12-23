using MediatR;
using TaskTrackerApp.Domain.DTOs.Auth;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Auth.Commands.LoginCommand;

public class LoginCommand : IRequest<Result<AuthResponse>>
{
    public string? Email { get; set; }

    public string? Tag { get; set; }

    public string Password { get; set; }
}