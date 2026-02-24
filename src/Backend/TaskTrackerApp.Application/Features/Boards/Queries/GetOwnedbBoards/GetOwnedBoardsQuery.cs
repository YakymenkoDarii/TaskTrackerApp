using MediatR;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetOwnedbBoards;

public class GetOwnedBoardsQuery : IRequest<Result<IEnumerable<BoardDto>>>
{
}