using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Application.Features.Boards.Commands.DeleteBoards;
public class DeleteBoardCommand : IRequest
{
    public int Id { get; set; }
}
