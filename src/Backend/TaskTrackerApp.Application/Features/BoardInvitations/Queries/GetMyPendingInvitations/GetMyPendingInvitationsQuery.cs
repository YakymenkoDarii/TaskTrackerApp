using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Queries.GetMyPendingInvitations;

public class GetMyPendingInvitationsQuery : IRequest<Result<IEnumerable<MyInvitationDto>>>
{
    public int UserId { get; set; }
}