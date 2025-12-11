using MediatR;

namespace TaskTrackerApp.Application.Features.Columns.Commands.UpdateColumns;

public class UpdateColumnCommand : IRequest
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int BoardId { get; set; }

    public int UpdatedBy { get; set; }
}