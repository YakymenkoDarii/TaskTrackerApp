namespace TaskTrackerApp.Domain.Errors.BoardMember;

public static class BoardMemberErrors
{
    public static readonly Error NotAuthorized = new(
        "BoardMember.NotAuthorized", "Only Board Admins can change member roles.");

    public static readonly Error NotFound = new(
        "BoardMember.NotFound", "Target member not found on this board.");

    public static readonly Error LastAdminCannotBeDemoted = new(
        "BoardMember.LastAdminCannotBeDemoted", "Cannot change role. The board must have at least one Admin.");
}