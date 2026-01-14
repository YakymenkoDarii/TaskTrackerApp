using MediatR;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardMembers.Commands.UpdateMemberRole;

public class UpdateMemberRoleCommand : IRequest<Result>
{
    public int BoardId { get; set; }

    public int MemberId { get; set; }

    public BoardRole Role { get; set; }
}