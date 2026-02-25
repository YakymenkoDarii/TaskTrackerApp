using MediatR;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetSharedWithMeBoards;

public class GetSharedWithMeBoardsQuery : IRequest<Result<IEnumerable<BoardDto>>>
{
}