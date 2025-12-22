using MediatR;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Application.Features.Boards.Commands.AddNewUser;

public class AddNewUserCommand : IRequest<int>
{
    public int UserId { get; set; }

    public int BoardId { get; set; }

    public BoardRole Role { get; set; }
}