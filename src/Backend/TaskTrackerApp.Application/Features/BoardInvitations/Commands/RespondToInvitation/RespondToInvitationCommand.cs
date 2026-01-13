using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Commands.RespondToInvitation;

public class RespondToInvitationCommand : IRequest<Result>
{
    public int InvitationId { get; set; }

    public bool IsAccepted { get; set; }
}