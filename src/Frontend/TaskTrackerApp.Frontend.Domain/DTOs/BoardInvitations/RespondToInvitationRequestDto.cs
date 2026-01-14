namespace TaskTrackerApp.Frontend.Domain.DTOs.BoardInvitations;

public class RespondToInvitationRequestDto
{
    public int InvitationId { get; set; }

    public bool IsAccepted { get; set; }
}