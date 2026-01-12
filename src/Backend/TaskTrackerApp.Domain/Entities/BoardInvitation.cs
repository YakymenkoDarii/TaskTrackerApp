using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.Entities;

public class BoardInvitation
{
    public int Id { get; set; }

    public int BoardId { get; set; }

    public Board Board { get; set; }

    public int SenderId { get; set; }

    public User? Sender { get; set; }

    public string InviteeEmail { get; set; }

    public int? InviteeId { get; set; }

    public User? Invitee { get; set; }

    public BoardRole Role { get; set; }

    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
}