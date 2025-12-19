using MediatR;
using TaskTrackerApp.Domain.DTOs.Auth;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Auth.Commands.SignupCommand;

public class SignupCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; }

    public string Password { get; set; }

    public string Tag { get; set; }

    public string DisplayName { get; set; }
}