namespace TaskTrackerApp.Frontend.Domain.Errors.User;

public class UserErrors
{
    public static readonly Error Unauthorized = new(
    "User.Unauthorized", "User is unauthorized.");

    public static readonly Error NotFound = new(
    "User.NotFound", "User is not found.");

    public static readonly Error PasswordMismatch = new(
    "User.PasswordMismatch", "Confirm and entered passwords are different.");

    public static readonly Error InvalidPassword = new(
    "User.InvalidPassword", "Old and new password differ.");

    public static readonly Error TagInUse = new(
    "User.TagInUse", "This tag is already in use by another user.");
}