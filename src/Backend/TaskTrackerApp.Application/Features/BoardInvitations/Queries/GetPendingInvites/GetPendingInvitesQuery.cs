using MediatR;
using TaskTrackerApp.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Queries.GetPendingInvites;

public class GetPendingInvitesQuery : IRequest<Result<IEnumerable<BoardInvitationDto>>>
{
    public int BoardId { get; set; }
}