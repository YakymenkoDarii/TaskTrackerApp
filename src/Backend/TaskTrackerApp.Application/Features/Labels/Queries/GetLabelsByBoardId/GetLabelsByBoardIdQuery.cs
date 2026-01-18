using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.DTOs.Labels;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Queries.GetLabelsByBoardId;

public class GetLabelsByBoardIdQuery : IRequest<Result<IEnumerable<LabelDto>>>
{
    public int BoardId { get; set; }
}