using Refit;
using System.Net;
using TaskTrackerApp.Frontend.Domain.Errors;

namespace TaskTrackerApp.Frontend.Domain.Results;

public static class ApiResponseExtensions
{
    public static Result<T> ToResult<T>(this IApiResponse<Result<T>> response)
    {
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result<T>.Failure(AuthErrors.AuthError);
            }

            return Result<T>.Failure(new Error(
                ClientErrors.NetworkError.Code,
                response.ReasonPhrase ?? "Unknown Network Error"));
        }
        var backendResult = response.Content;

        if (backendResult == null)
        {
            return Result<T>.Failure(new Error("SerializationError", "Response was empty"));
        }

        if (!backendResult.IsSuccess)
        {
            return Result<T>.Failure(backendResult.Error);
        }

        return Result<T>.Success(backendResult.Value);
    }

    public static Result ToResult(this IApiResponse<Result> response)
    {
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Failure(AuthErrors.AuthError);

            return Result.Failure(new Error(ClientErrors.NetworkError.Code, response.ReasonPhrase ?? "Network Error"));
        }

        var backendResult = response.Content;

        if (backendResult == null)
        {
            return Result.Failure(new Error("SerializationError", "Response was empty"));
        }

        if (!backendResult.IsSuccess)
        {
            return Result.Failure(backendResult.Error);
        }

        return Result.Success();
    }
}