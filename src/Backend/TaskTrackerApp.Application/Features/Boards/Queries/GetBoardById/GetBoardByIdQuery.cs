using MediatR;
using TaskTrackerApp.Domain.DTOs.Board;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetBoardById;

public class GetBoardByIdQuery : IRequest<BoardDto>
{
    public int Id { get; set; }

    public GetBoardByIdQuery(int id)
    {
        Id = id;
    }
}