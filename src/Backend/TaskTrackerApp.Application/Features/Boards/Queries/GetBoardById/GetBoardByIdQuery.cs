using MediatR;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetBoardById;

public class GetBoardByIdQuery : IRequest<Result<BoardDto>>
{
    public int Id { get; set; }

    public int CurrentUserId { get; set; }
}