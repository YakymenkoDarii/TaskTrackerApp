using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Application.Features.Boards.Commands.AddNewUser;
public class AddNewUserCommand :IRequest<int>
{
    public int UserId { get; set; }

    public int BoardId { get; set; }

    public string Role { get; set; }
}
