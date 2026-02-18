using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Boards;

public class BoardsService : IBoardsService
{
    private readonly IBoardsApi _boardsApi;

    public BoardsService(IBoardsApi boardsApi)
    {
        _boardsApi = boardsApi;
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

    public async Task<Result<int>> CreateAsync(CreateBoardDto board)
    {
        try
        {
            var response = await _boardsApi.CreateAsync(board);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<int>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            await _boardsApi.DeleteAsync(id);
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

    public async Task<Result<BoardDto>> GetBoardByIdAsync(int boardId)
    {
        try
        {
            var board = await _boardsApi.GetByIdAsync(boardId);
            return board.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<BoardDto>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<BoardDto>.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> UpdateAsync(int id, UpdateBoardDto dto)
    {
        try
        {
            var response = await _boardsApi.UpdateAsync(id, dto);
            return response.ToResult();
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

    public async Task<Result> ArchiveBoardAsync(int boardId)
    {
        try
        {
            var response = await _boardsApi.ArchiveBoardAsync(boardId);
            return response.ToResult();
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

    public async Task<Result<IEnumerable<ArchivedBoardDto>>> GetArchivedAsync()
    {
        try
        {
            var response = await _boardsApi.GetArchivedAsync();
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<IEnumerable<ArchivedBoardDto>>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ArchivedBoardDto>>.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> UnArchiveBoardAsync(int boardId)
    {
        try
        {
            var response = await _boardsApi.UnArchiveBoardAsync(boardId);
            return response.ToResult();
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

    public async Task<Result> TransferOwnershipAsync(int boardId, int userId)
    {
        try
        {
            var response = await _boardsApi.TransferOwnership(boardId, userId);
            return response.ToResult();
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