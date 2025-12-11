using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Application.Features.Boards.Commands.UpdateBoards;
public class UpdateBoardCommand : IRequest
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int UpdatedBy { get; set; }
}
