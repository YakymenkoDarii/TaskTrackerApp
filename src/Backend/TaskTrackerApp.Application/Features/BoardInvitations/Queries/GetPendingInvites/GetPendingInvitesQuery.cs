using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Queries.GetPendingInvites;

public class GetPendingInvitesQuery : IRequest<Result<IEnumerable<BoardInvitationDto>>>
{
    public int BoardId { get; set; }
}