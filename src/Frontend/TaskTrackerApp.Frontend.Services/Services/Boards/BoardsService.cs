using Refit;
using System.Text.Json;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Boards;

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
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<IEnumerable<BoardDto>>.Failure(ClientErrors.NetworkError);
        }
    }

    public async Task<Result> CreateAsync(CreateBoardDto board)
    {
        try
        {
            await _boardsApi.CreateAsync(board);
            return Result.Success();
        }
        catch (ApiException ex)
        {
            return Result.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("UnknownError", ex.Message));
        }
    }
}