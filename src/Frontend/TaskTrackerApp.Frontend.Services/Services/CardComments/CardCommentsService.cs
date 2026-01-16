using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.CardComments;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.CardComments;

public class CardCommentsService : ICardCommentsService
{
    private readonly ICardCommentsApi _api;

    public CardCommentsService(ICardCommentsApi api)
    {
        _api = api;
    }

    public async Task<Result> CreateComment(CreateCardCommentDto createDto)
    {
        try
        {
            var response = await _api.CreateCommentAsync(createDto);
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

    public async Task<Result> DeleteComment(int id)
    {
        try
        {
            var response = await _api.DeleteCommentAsync(id);
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

    public async Task<Result<IEnumerable<CardCommentDto>>> GetCommentsByCardId(int cardId)
    {
        try
        {
            var response = await _api.GetCommentsByCardIdasync(cardId);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<IEnumerable<CardCommentDto>>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CardCommentDto>>.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> UpdateComment(UpdateCardCommentDto updateDto)
    {
        try
        {
            var response = await _api.UpdateCommentAsync(updateDto);
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