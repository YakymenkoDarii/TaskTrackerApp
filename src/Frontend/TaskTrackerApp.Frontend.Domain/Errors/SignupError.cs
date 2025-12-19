namespace TaskTrackerApp.Frontend.Domain.Errors;

public static class SignupError
{
    public static readonly Error EmailInUse = new(
        "Signup.EmailInUse", "This email is already used");

    public static readonly Error TagInUse = new(
    "Signup.TagInUse", "This tag is already used");
}