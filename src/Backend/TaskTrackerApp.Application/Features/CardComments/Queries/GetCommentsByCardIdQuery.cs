using MediatR;
using TaskTrackerApp.Domain.DTOs.CardComment;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.CardComments.Queries;

public class GetCommentsByCardIdQuery : IRequest<Result<IEnumerable<CardCommentDto>>>
{
    public int CardId { get; set; }
}