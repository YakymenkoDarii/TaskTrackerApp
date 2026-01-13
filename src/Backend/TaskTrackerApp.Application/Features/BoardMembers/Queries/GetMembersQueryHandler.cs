using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardMembers.Queries;

public class GetMembersQueryHandler : IRequestHandler<GetMembersQuery, Result<IEnumerable<BoardMemberDto>>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetMembersQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<BoardMemberDto>>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var members = await uow.BoardMembersRepository.GetByBoardId(request.BoardId);

        var memberDtos = members.Select(m => new BoardMemberDto
        {
            UserId = m.UserId,
            Name = m.User?.DisplayName,
            AvatarUrl = m.User?.AvatarUrl,
            Role = m.Role.ToString()
        });

        return Result<IEnumerable<BoardMemberDto>>.Success(memberDtos);
    }
}