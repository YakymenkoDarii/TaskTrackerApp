using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.DTOs.Labels;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.CreateLabel;

public class CreateLabelCommand : IRequest<Result<LabelDto>>
{
    public CreateLabelDto CreateLabel { get; set; }
}