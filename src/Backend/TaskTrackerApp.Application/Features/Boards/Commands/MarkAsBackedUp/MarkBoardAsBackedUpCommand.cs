using MediatR;

namespace TaskTrackerApp.Application.Features.Boards.Commands.MarkAsBackedUp;

public class MarkBoardAsBackedUpCommand : IRequest
{
    public int BoardId { get; set; }

    public MarkBoardAsBackedUpCommand(int boardId)
    {
        BoardId = boardId;
    }
}