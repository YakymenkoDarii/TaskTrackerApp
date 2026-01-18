using MediatR;
using TaskTrackerApp.Domain.DTOs.Labels;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.CreateLabel;

public class CreateLabelCommand : IRequest<Result<LabelDto>>
{
    public CreateLabelDto CreateLabel { get; set; }
}