using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.UnArchiveBoard;

public class UnArchiveBoardCommand : IRequest<Result>
{
    public int BoardId { get; set; }
}