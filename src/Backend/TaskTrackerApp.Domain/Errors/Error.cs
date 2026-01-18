using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Domain.Errors;
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static implicit operator Result(Error error) => Result.Failure(error);

    public static readonly Error NotFound = new(
        "NotFound", "Not found.");
}