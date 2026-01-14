namespace TaskTrackerApp.Domain.Errors.Auth;

public static class AuthErrors
{
    public static readonly Error NotAuthenticated = new(
        "Auth.NotAuthenticated", "User is not authenticated.");
}