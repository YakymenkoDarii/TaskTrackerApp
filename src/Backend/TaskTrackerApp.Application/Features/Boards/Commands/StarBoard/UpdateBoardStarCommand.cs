using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.StarBoard;

public class UpdateBoardStarCommand : IRequest<Result>
{
    public int BoardId { get; set; }

    public bool IsStarred { get; set; }
}