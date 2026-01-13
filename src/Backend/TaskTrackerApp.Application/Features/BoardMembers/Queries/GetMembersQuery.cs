using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardMembers.Queries;

public class GetMembersQuery : IRequest<Result<IEnumerable<BoardMemberDto>>>
{
    public int BoardId { get; set; }
}