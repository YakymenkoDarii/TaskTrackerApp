using Refit;
using System.Net;
using System.Text.Json;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;

public static class ApiResponseExtensions
{
    public static Result<T> ToResult<T>(this IApiResponse<Result<T>> response)
    {
        if (response.IsSuccessStatusCode && response.Content is not null)
        {
            return response.Content.IsSuccess
                ? Result<T>.Success(response.Content.Value)
                : Result<T>.Failure(response.Content.Error);
        }

        if (response.Error?.Content is not null)
        {
            try
            {
                using var doc = JsonDocument.Parse(response.Error.Content);
                var root = doc.RootElement;

                if (root.TryGetProperty("code", out var codeProp) &&
                    root.TryGetProperty("message", out var msgProp))
                {
                    var code = codeProp.GetString() ?? "Unknown";
                    var message = msgProp.GetString() ?? "Unknown error";

                    var specificError = new Error(code, message);
                    return Result<T>.Failure(specificError);
                }
            }
            catch
            {
                Console.WriteLine("[ToResult] Failed to parse error JSON manually.");
            }
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Result<T>.Failure(AuthErrors.AuthError);
        }

        return Result<T>.Failure(new Error(
            ClientErrors.NetworkError.Code,
            response.ReasonPhrase ?? "Unknown Network Error"));
    }

    public static Result ToResult(this IApiResponse<Result> response)
    {
        if (response.IsSuccessStatusCode && response.Content is not null)
        {
            return response.Content.IsSuccess
                ? Result.Success()
                : Result.Failure(response.Content.Error);
        }

        if (response.Error?.Content is not null)
        {
            try
            {
                using var doc = JsonDocument.Parse(response.Error.Content);
                var root = doc.RootElement;

                if (root.TryGetProperty("code", out var codeProp) &&
                    root.TryGetProperty("message", out var msgProp))
                {
                    return Result.Failure(new Error(codeProp.GetString(), msgProp.GetString()));
                }
            }
            catch
            {
                Console.WriteLine("[ToResult] Failed to parse error JSON manually.");
            }
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return Result.Failure(AuthErrors.AuthError);

        return Result.Failure(new Error(ClientErrors.NetworkError.Code, response.ReasonPhrase ?? "Network Error"));
    }
}