using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.AddLabelToCard;

public class AddLabelToCardCommand : IRequest<Result>
{
    public int CardId { get; set; }

    public int LabelId { get; set; }
}