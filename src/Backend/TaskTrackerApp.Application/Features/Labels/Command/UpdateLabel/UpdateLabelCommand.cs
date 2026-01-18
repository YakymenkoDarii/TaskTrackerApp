using MediatR;
using TaskTrackerApp.Domain.DTOs.Labels;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.UpdateLabel;

public class UpdateLabelCommand : IRequest<Result<LabelDto>>
{
    public LabelDto Dto { get; set; }
}