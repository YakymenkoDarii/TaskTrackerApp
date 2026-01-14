using MediatR;
using TaskTrackerApp.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardMembers.Queries;

public class GetMembersQuery : IRequest<Result<IEnumerable<BoardMemberDto>>>
{
    public int BoardId { get; set; }
}