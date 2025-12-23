using Refit;
using System.Text.Json;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services;

public class BoardsService : IBoardsService
{
    private readonly IBoardsApi _boardsApi;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public BoardsService(IBoardsApi boardsApi)
    {
        _boardsApi = boardsApi;
        _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<Result<IEnumerable<BoardDto>>> GetAllAsync()
    {
        try
        {
            var response = await _boardsApi.GetAllAsync();
            return ParseResponse(response);
        }
        catch (ApiException ex)
        {
            return Result<IEnumerable<BoardDto>>.Failure(new Error("Client.Network", "Network error."));
        }
    }

    private Result<IEnumerable<BoardDto>> ParseResponse(IApiResponse<IEnumerable<BoardDto>> response)
    {
        if (response.IsSuccessStatusCode && response.Content is not null)
        {
            return Result<IEnumerable<BoardDto>>.Success(response.Content);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return Result<IEnumerable<BoardDto>>.Failure(new Error("Auth.Unauthorized", "Please log in again."));
        }

        if (response.Error?.Content is null)
        {
            return Result<IEnumerable<BoardDto>>.Success(null);
        }

        try
        {
            if (response.Error?.Content != null)
            {
                var backendError = JsonSerializer.Deserialize<Error>(
                    response.Error.Content,
                    _jsonSerializerOptions);

                if (backendError != null) return Result<IEnumerable<BoardDto>>.Failure(backendError);
            }
        }
        catch (JsonException)
        {
        }

        return Result<IEnumerable<BoardDto>>.Failure(
            new Error("Client.Server", response.ReasonPhrase ?? "Unknown server error"));
    }
}