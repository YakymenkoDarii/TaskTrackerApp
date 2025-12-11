using MediatR;

namespace TaskTrackerApp.Application.Features.Users.Commands.CreateUsers;

public class CreateUserCommand : IRequest<int>
{
    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public string Tag { get; set; }

    public string DisplayName { get; set; }
}