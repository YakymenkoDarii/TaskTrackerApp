using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface ICardsService
{
    Task<Result<IEnumerable<CardDto>>> GetCardsByColumnId(int columnId);

    Task<Result<int>> CreateCardAsync(CreateCardDto cardDto);

    Task<Result> DeleteCardAsync(int id);

    Task<Result<CardDto>> UpdateAsync(int id, UpdateCardDto updateCardDto);
}