using MediatR;
using TaskTrackerApp.Application.Features.Behaviors.Interfaces.Boards;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardMembers.Commands.DeleteMember;

public class DeleteMemberCommand : IRequest<Result>
{
    public int BoardId { get; set; }

    public int MemberId { get; set; }
}