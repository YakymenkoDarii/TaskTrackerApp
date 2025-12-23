using MediatR;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetAllBoards;

public class GetAllBoardsQuery : IRequest<Result<IEnumerable<BoardDto>>>
{
    public int UserId { get; set; }

    public GetAllBoardsQuery(int userId)
    {
        UserId = userId;
    }
}