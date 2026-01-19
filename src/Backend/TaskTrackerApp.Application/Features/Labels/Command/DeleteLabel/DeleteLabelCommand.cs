using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.DeleteLabel;

public class DeleteLabelCommand : IRequest<Result>
{
    public int LabelId { get; set; }
}