using MediatR;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetArchivedBoards;

public class GetArchivedBoardsQuery : IRequest<Result<IEnumerable<ArchivedBoardDto>>>
{
}