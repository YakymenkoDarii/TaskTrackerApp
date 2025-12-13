using MediatR;

namespace TaskTrackerApp.Application.Features.Boards.Commands.UpdateBoards;

public class UpdateBoardCommand : IRequest
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int UpdatedById { get; set; }
}