using MediatR;
using TaskTrackerApp.Domain.DTOs.Board;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetFullBoardDetails;

public class GetFullBoardDetailsQuery : IRequest<BoardExportDto>
{
    public int BoardId { get; set; }
}