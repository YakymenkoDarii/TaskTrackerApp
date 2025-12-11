using MediatR;

namespace TaskTrackerApp.Application.Features.Users.Commands.UpdateUsers;

public class UpdateUserCommand : IRequest
{
    public int Id { get; set; }

    public string Tag { get; set; }

    public string PasswordHash { get; set; }

    public string DisplayName { get; set; }

    public string AvatarUrl { get; set; }
}