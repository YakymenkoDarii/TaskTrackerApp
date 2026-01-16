using TaskTrackerApp.Frontend.Domain.DTOs.CardComments;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface ICardCommentsService
{
    Task<Result> CreateComment(CreateCardCommentDto createDto);

    Task<Result> DeleteComment(int id);

    Task<Result<IEnumerable<CardCommentDto>>> GetCommentsByCardId(int cardId);

    Task<Result> UpdateComment(UpdateCardCommentDto updateDto);
}