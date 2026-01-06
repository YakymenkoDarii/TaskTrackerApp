using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface ICardsApi
{
    [Get("/api/Cards/{columnId}")]
    Task<IApiResponse<Result<IEnumerable<CardDto>>>> GetByColumnIdAsync(int columnId);

    [Post("/api/Cards")]
    Task<IApiResponse<Result>> CreateAsync(CreateCardDto columnDto);

    [Delete("/api/Cards")]
    Task<IApiResponse<Result>> DeleteAsync(int cardId);

    [Put("/api/Cards/{id}")]
    Task<IApiResponse<Result<CardDto>>> UpdateAsync(int id, UpdateCardDto cardDto);
}