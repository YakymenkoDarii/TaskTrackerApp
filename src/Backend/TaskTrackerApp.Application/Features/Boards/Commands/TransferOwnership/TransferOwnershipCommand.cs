using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.TransferOwnership;

public class TransferOwnershipCommand : IRequest<Result>
{
    public int BoardId { get; set; }

    public int TransferUserId { get; set; }
}