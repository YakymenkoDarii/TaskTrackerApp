using MediatR;

namespace TaskTrackerApp.Application.Features.Boards.Commands.CreateBoards;

public class CreateBoardCommand : IRequest<int>
{
    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int CreatedById { get; set; }
}