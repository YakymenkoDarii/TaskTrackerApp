using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Meeting;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Meeting.Queries.GetMeetingParticipantWithPeerId;

internal class GetMeetingParticipantWithPeerIdQueryHandler : IRequestHandler<GetMeetingParticipantWithPeerIdQuery, Result<MeetingParticipant>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetMeetingParticipantWithPeerIdQueryHandler(ICurrentUserService currentUserService, IUnitOfWorkFactory uowFactory)
    {
        _currentUserService = currentUserService;
        _uowFactory = uowFactory;
    }

    public async Task<Result<MeetingParticipant>> Handle(GetMeetingParticipantWithPeerIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return UserErrors.Unauthorized;
        }

        var uow = _uowFactory.Create();

        var user = await uow.UserRepository.GetById(userId.Value);
        if (user == null)
        {
            return UserErrors.NotFound;
        }

        var participant = new MeetingParticipant
        {
            PeerId = request.PeerId,
            UserId = userId.Value,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl,
            IsMuted = request.IsMuted,
            IsVideoOff = request.IsVideoOff,
        };

        return participant;
    }
}