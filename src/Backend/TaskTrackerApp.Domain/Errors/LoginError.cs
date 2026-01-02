namespace TaskTrackerApp.Domain.Errors;

public static class LoginError
{
    public static readonly Error InvalidPassword = new(
        "Login.InvalidPassword", "Invalid password.");

    public static readonly Error UserNotFound = new(
        "Login.UserNotFound", "No user found with this email or tag.");

    public static readonly Error InvalidRefreshToken = new(
    "Login.InvalidRefreshToken", "Invalid refresh token.");

    public static readonly Error RefreshTokenExpired = new(
    "Login.RefreshTokenExpired", "Refresh token has exipered.");
}