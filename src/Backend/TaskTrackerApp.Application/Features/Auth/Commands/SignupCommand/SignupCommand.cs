using MediatR;
using TaskTrackerApp.Domain.DTOs.Auth;

namespace TaskTrackerApp.Application.Features.Auth.Commands.SignupCommand;

public class SignupCommand : IRequest<AuthResponse>
{
    public string Email { get; set; }

    public string Password { get; set; }

    public string Tag { get; set; }

    public string DisplayName { get; set; }
}