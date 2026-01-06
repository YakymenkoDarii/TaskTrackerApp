using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Cards;

public class CardsService : ICardsService
{
    private readonly ICardsApi _cardsApi;

    public CardsService(ICardsApi cardsApi)
    {
        _cardsApi = cardsApi;
    }

    public async Task<Result> CreateCardAsync(CreateCardDto cardDto)
    {
        try
        {
            await _cardsApi.CreateAsync(cardDto);
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

    public async Task<Result> DeleteCardAsync(int id)
    {
        try
        {
            await _cardsApi.DeleteAsync(id);
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

    public async Task<Result<IEnumerable<CardDto>>> GetCardsByColumnId(int columnId)
    {
        try
        {
            var response = await _cardsApi.GetByColumnIdAsync(columnId);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<IEnumerable<CardDto>>.Failure(ClientErrors.NetworkError);
        }
    }
}