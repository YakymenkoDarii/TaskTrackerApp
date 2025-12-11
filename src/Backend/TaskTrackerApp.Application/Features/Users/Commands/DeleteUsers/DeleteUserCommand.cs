using MediatR;

namespace TaskTrackerApp.Application.Features.Users.Commands.DeleteUsers;

public class DeleteUserCommand : IRequest
{
    public int Id { get; set; }
}