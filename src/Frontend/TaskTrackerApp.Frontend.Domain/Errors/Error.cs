using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Domain.Errors;
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static implicit operator Result(Error error) => Result.Failure(error);
}