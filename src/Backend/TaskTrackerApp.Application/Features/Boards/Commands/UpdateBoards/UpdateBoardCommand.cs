using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.UpdateBoards;

public class UpdateBoardCommand : IRequest<Result>
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int UpdatedById { get; set; }
}