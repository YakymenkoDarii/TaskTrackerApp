using Refit;
using System.Net;
using System.Text.Json;
using TaskTrackerApp.Frontend.Domain.Errors;

namespace TaskTrackerApp.Frontend.Domain.Results;

public static class ApiResponseExtensions
{
    public static Result<T> ToResult<T>(this IApiResponse<T> response, JsonSerializerOptions jsonOptions = null)
    {
        if (response.Error?.Content != null)
        {
            try
            {
                var options = jsonOptions ?? new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var backendError = JsonSerializer.Deserialize<Error>(response.Error.Content, options);

                if (backendError != null)
                {
                    return Result<T>.Failure(backendError);
                }
            }
            catch (JsonException)
            {
            }
        }

        if (response.IsSuccessStatusCode)
        {
            return Result<T>.Success(response.Content);
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Result<T>.Failure(AuthErrors.AuthError);
        }

        return Result<T>.Failure(
            new Error(
                ClientErrors.NetworkError.Code,
                response.ReasonPhrase ?? "Unknown Network Error"
            ));
    }
}