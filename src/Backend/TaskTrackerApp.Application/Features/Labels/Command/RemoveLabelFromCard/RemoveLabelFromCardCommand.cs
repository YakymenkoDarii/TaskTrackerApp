using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.RemoveLabelFromCard;

public class RemoveLabelFromCardCommand : IRequest<Result>
{
    public int CardId { get; set; }

    public int LabelId { get; set; }
}