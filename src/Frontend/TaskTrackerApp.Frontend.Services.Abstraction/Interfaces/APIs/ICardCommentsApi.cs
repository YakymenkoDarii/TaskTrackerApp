using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.CardComments;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface ICardCommentsApi
{
    [Get("/api/CardComments/{cardId}")]
    Task<IApiResponse<Result<IEnumerable<CardCommentDto>>>> GetCommentsByCardIdasync(int cardId);

    [Post("/api/CardComments/create")]
    Task<IApiResponse<Result>> CreateCommentAsync([Body] MultipartFormDataContent content);

    [Delete("/api/CardComments/{id}")]
    Task<IApiResponse<Result>> DeleteCommentAsync(int id);

    [Put("/api/CardComments/update")]
    Task<IApiResponse<Result>> UpdateCommentAsync([Body] MultipartFormDataContent content);
}