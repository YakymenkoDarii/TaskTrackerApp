using MediatR;
using TaskTrackerApp.Domain.DTOs.Labels;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Queries.GetLabelsByBoardId;

public class GetLabelsByBoardIdQuery : IRequest<Result<IEnumerable<LabelDto>>>
{
    public int BoardId { get; set; }
}