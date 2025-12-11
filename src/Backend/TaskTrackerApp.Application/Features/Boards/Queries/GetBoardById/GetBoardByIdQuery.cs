using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
