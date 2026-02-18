using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.TransferOwnership;

public class TransferOwnershipCommand : IRequest<Result>
{
    public int BoardId { get; set; }

    public int TransferUserId { get; set; }
}