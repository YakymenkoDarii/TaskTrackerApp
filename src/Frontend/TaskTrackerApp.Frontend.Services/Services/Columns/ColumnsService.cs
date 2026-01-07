using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Columns;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Columns;

public class ColumnsService : IColumnsService
{
    private readonly IColumnsApi _columnsApi;

    public ColumnsService(IColumnsApi columnsApi)
    {
        _columnsApi = columnsApi;
    }

    public async Task<Result<int>> CreateColumnAsync(CreateColumnDto columnDto)
    {
        try
        {
            var response = await _columnsApi.CreateAsync(columnDto);
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

    public async Task<Result> DeleteColumnAsync(int id)
    {
        try
        {
            await _columnsApi.DeleteAsync(id);
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

    public async Task<Result<IEnumerable<ColumnDto>>> GetByBoardIdAsync(int boardId)
    {
        var columns = await _columnsApi.GetByBoardIdAsync(boardId);

        return columns.ToResult();
    }

    public async Task<Result> UpdateColumnAsync(int id, UpdateColumnDto columnDto)
    {
        var result = await _columnsApi.UpdateAsync(id, columnDto);

        return result.ToResult();
    }
}