using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Users.Commands.UpdateUsers;

public class UpdateUserCommand : IRequest<Result>
{
    public string Tag { get; set; }

    public string DisplayName { get; set; }
}