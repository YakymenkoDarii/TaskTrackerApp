using MediatR;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.UpdateBoardTheme;

public class UpdateBoardThemeCommand : IRequest<Result>
{
    public int BoardId { get; set; }

    public BoardThemeColor NewColor { get; set; }
}