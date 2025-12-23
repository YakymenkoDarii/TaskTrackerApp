namespace TaskTrackerApp.Frontend.Domain.Errors;

public static class AuthErrors
{
    public static Error AuthError = new(
        "Auth.Unauthorized", "Please log in again.");
}