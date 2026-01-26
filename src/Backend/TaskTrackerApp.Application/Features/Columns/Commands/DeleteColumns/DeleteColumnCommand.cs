using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Columns.Commands.DeleteColumns;

public class DeleteColumnCommand : IRequest<Result>
{
    public int Id { get; set; }
}