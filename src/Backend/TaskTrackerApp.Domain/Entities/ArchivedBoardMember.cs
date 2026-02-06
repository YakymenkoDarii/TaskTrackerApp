using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.Entities;

public class ArchivedBoardMember
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ArchivedBoardId { get; set; }

    public BoardRole Role { get; set; }

    public User User { get; set; }

    public ArchivedBoard ArchivedBoard { get; set; }
}