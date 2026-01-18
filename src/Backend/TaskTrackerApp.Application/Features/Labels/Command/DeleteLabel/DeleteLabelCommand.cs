using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.DeleteLabel;

public class DeleteLabelCommand : IRequest<Result>
{
    public int LabelId { get; set; }
}