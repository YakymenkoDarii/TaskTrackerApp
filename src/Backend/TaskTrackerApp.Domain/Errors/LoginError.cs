namespace TaskTrackerApp.Domain.Errors;

public static class LoginError
{
    public static readonly Error InvalidPassword = new(
        "Login.InvalidPassword", "Invalid password.");

    public static readonly Error UserNotFound = new(
        "Login.UserNotFound", "No user found with this email or tag.");
}