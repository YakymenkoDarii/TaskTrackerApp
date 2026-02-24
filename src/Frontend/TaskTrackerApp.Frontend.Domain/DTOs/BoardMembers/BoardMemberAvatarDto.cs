namespace TaskTrackerApp.Frontend.Domain.DTOs.BoardMembers;

public class BoardMemberAvatarDto
{
    public int UserId { get; set; }

    public string DisplayName { get; set; }

    public string? AvatarUrl { get; set; }
}