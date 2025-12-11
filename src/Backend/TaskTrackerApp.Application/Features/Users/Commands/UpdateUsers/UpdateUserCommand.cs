using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Application.Features.Users.Commands.UpdateUsers;
public class UpdateUserCommand : IRequest
{
    public int Id { get; set; }

    public required string Tag { get; set; }

    public required string PasswordHash { get; set; }

    public required string DisplayName { get; set; }

    public required string AvatarUrl { get; set; }
}
