using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Labels;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Labels;

public class LabelService : ILabelService
{
    private readonly ILabelApi _api;

    public LabelService(ILabelApi api)
    {
        _api = api;
    }

    public async Task<Result> AddLabelToCardAsync(int cardId, int id)
    {
        try
        {
            var response = await _api.AddLabelToCardAsync(cardId, id);
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

    public async Task<Result<LabelDto>> CreateLabelAsync(CreateLabelDto createDto)
    {
        try
        {
            var response = await _api.CreateLabelAsync(createDto);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<LabelDto>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<LabelDto>.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> DeleteLabelAsync(int id)
    {
        try
        {
            var response = await _api.DeleteLabelAsync(id);
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

    public async Task<Result<IEnumerable<LabelDto>>> GetLabelsByBoardIdAsync(int boardId)
    {
        try
        {
            var response = await _api.GetLabelsByBoardIdAsync(boardId);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<IEnumerable<LabelDto>>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<LabelDto>>.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> RemoveLabelFromCardAsync(int cardId, int id)
    {
        try
        {
            var response = await _api.RemoveLabelFromCardAsync(cardId, id);
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

    public async Task<Result<LabelDto>> UpdateLabelAsync(LabelDto updateDto)
    {
        try
        {
            var response = await _api.UpdateLabelAsync(updateDto);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<LabelDto>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<LabelDto>.Failure(new Error("UnknownError", ex.Message));
        }
    }
}