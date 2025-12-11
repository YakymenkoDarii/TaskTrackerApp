using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.DTOs.Board;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetAllBoards;
public class GetAllBoardsQuery : IRequest<IEnumerable<BoardDto>>
{
    public int UserId { get; set; }

    public GetAllBoardsQuery(int userId)
    {
        UserId = userId;
    }
}
