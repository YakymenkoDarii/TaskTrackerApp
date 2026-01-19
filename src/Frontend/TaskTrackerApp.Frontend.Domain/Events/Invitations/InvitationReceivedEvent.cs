using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.Events.Invitations;
public record InvitationReceivedEvent(
    int InvitationId,
    int BoardId,
    string Sender,
    string BoardName
);