using MediatR;
using TaskTrackerApp.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Queries.GetMyPendingInvitations;

public class GetMyPendingInvitationsQuery : IRequest<Result<IEnumerable<MyInvitationDto>>>
{
    public int UserId { get; set; }
}