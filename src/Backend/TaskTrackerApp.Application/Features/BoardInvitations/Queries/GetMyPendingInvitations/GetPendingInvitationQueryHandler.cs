using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Queries.GetMyPendingInvitations;

public class GetMyPendingInvitationsQueryHandler
    : IRequestHandler<GetMyPendingInvitationsQuery, Result<IEnumerable<MyInvitationDto>>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetMyPendingInvitationsQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<MyInvitationDto>>> Handle(
        GetMyPendingInvitationsQuery request,
        CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var invitations = await uow.BoardInvitationsRepository.GetMyPendingAsync(request.UserId);

        var dtos = invitations.Select(i => new MyInvitationDto
        {
            Id = i.Id,
            BoardId = i.BoardId,
            BoardTitle = i.Board.Title,
            SenderName = i.Sender?.DisplayName ?? "Unknown",
            SenderAvatarUrl = i.Sender?.AvatarUrl,
            SentAt = DateTime.UtcNow
        });

        return Result<IEnumerable<MyInvitationDto>>.Success(dtos);
    }
}