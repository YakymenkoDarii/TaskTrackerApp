using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface IBoardMembersRepository : IRepository<BoardMember, int>
{
    Task<bool> ChangeRoleAsync(int boardId, int userId, BoardRole role);

    Task<bool> ExistsAsync(int boardId, int userId);

    Task<IEnumerable<BoardMember>> GetByBoardId(int boardId);

    Task<IEnumerable<BoardMember>> GetByUserIdAsync(int userId);
}