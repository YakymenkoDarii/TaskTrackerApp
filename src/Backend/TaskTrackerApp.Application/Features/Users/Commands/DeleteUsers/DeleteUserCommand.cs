using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Application.Features.Users.Commands.DeleteUsers;
public class DeleteUserCommand : IRequest
{
    public int Id { get; set; }
}
