using MediatR;

namespace TaskTrackerApp.Application.Features.Columns.Commands.CreateColumns;

public class CreateColumnCommand : IRequest<int>
{
    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int BoardId { get; set; }
}