using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.AddLabelToCard;

public class AddLabelToCardCommand : IRequest<Result>
{
    public int CardId { get; set; }

    public int LabelId { get; set; }
}