using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface ICardsApi
{
    [Get("/api/Cards/{columnId}")]
    Task<IApiResponse<Result<IEnumerable<CardDto>>>> GetByColumnIdAsync(int columnId);

    [Post("/api/Cards")]
    Task<IApiResponse<Result<int>>> CreateAsync(CreateCardDto columnDto);

    [Delete("/api/Cards")]
    Task<IApiResponse<Result>> DeleteAsync(int cardId);

    [Put("/api/Cards/{id}")]
    Task<IApiResponse<Result<CardDto>>> UpdateAsync(int id, UpdateCardDto cardDto);

    [Get("/api/Cards/upcoming")]
    Task<IApiResponse<Result<IEnumerable<UpcomingCardDto>>>> GetUpcoming(DateTime weekStart, DateTime weekEnd, bool includeOverdue);

    [Put("/api/Cards/status/{id}")]
    Task<IApiResponse<Result>> UpdateStatus(int id, bool isCompleted);

    [Get("/api/Cards/search")]
    Task<ApiResponse<Result<IEnumerable<CardDto>>>> SearchAsync([Query] string query, [Query] int? boardId, [Query] int? assigneeId);
}