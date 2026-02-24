using MediatR;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.CreateBoards;

public class CreateBoardCommand : IRequest<Result<int>>
{
    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int CreatedById { get; set; }

    public BoardThemeColor ThemeColor { get; set; }
}