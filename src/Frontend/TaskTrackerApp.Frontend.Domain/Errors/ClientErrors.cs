namespace TaskTrackerApp.Frontend.Domain.Errors;

public static class ClientErrors
{
    public const string NetworkErrorCode = "Client.Network";

    public const string UnknownNetworkErrorCode = "Client.Server";

    public static Error NetworkError = new(
        "Client.Network", "Network error.");

    public static Error UnknownNetworkError = new(
        "Client.Server", "Unknown network error.");
}