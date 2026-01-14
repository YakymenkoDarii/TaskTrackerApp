using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Queries.GetPendingInvites;

public class GetPendingInvitesQueryHandler : IRequestHandler<GetPendingInvitesQuery, Result<IEnumerable<BoardInvitationDto>>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetPendingInvitesQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<BoardInvitationDto>>> Handle(GetPendingInvitesQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var invites = await uow.BoardInvitationsRepository.GetPendingInvitationsAsync(request.BoardId);

        var dtos = invites.Select(x => new BoardInvitationDto
        {
            Id = x.Id,
            BoardId = x.Id,
            InviteeId = x.InviteeId,
            InviteeEmail = x.InviteeEmail,
            InviteeAvatarUrl = x.Invitee.AvatarUrl,
            Role = x.Role.ToString(),
            Status = x.Status.ToString(),
        }).ToList();

        return dtos;
    }
}