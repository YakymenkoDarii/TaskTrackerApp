using MediatR;

namespace TaskTrackerApp.Application.Features.Columns.Commands.DeleteColumns;

public class DeleteColumnCommand : IRequest
{
    public int Id { get; set; }
}