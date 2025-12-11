using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Application.Features.Boards.Commands.CreateBoards;
public class CreateBoardCommand : IRequest<int>
{
    public required string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int CreatedBy { get; set; }
}
