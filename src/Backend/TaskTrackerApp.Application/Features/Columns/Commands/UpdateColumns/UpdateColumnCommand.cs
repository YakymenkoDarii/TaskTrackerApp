using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Columns.Commands.UpdateColumns;

public class UpdateColumnCommand : IRequest<Result>
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int BoardId { get; set; }

    public int UpdatedById { get; set; }

    public int Position { get; set; }
}