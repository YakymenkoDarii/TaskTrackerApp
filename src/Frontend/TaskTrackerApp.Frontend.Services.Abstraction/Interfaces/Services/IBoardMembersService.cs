using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface IBoardMembersService
{
    Task<Result<IEnumerable<BoardMemberDto>>> GetMembersAsync(int boardId);

    // Task<Result> RemoveMemberAsync(int boardId, int userId);
    // Task<Result> UpdateMemberRoleAsync(int boardId, int userId, BoardRole newRole);
}