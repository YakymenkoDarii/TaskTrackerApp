using MediatR;
using TaskTrackerApp.Domain.DTOs.Meeting;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Meeting.Queries.GetMeetingParticipantWithPeerId;

public class GetMeetingParticipantWithPeerIdQuery : IRequest<Result<MeetingParticipant>>
{
    public string PeerId { get; set; }

    public bool IsMuted { get; set; }

    public bool IsVideoOff { get; set; }
}