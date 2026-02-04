using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.ArchiveBoard;

public class ArchiveBoardCommand : IRequest<Result>
{
    public int BoardId { get; set; }
}