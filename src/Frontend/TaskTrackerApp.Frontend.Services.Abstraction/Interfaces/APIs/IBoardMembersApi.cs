using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface IBoardMembersApi
{
    [Get("/api/BoardMembers/{boardId}")]
    Task<IApiResponse<Result<IEnumerable<BoardMemberDto>>>> GetMembersAsync(int boardId);
}