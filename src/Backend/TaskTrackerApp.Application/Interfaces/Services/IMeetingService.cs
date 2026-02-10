using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.DTOs.Meeting;

namespace TaskTrackerApp.Application.Interfaces.Services;

public interface IMeetingService
{
    MeetingDto? GetMeeting(int boardId);

    MeetingDto StartOrJoinMeeting(int boardId, string peerId);

    void LeaveMeeting(int boardId, string peerId);
}