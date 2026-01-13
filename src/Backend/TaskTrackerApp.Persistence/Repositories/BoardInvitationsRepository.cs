using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class BoardInvitationsRepository : Repository<BoardInvitation, int>, IBoardInvitationsRepository
{
    public BoardInvitationsRepository(TaskTrackerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BoardInvitation>> GetPendingInvitationsAsync(int boardId)
    {
        return await _dbSet
            .Include(i => i.Board)
            .Include(i => i.Invitee)
            .Where(i => i.BoardId == boardId && i.Status == InvitationStatus.Pending)
            .ToListAsync();
    }

    public async Task<bool> IsInvitationPendingAsync(int boardId, string email)
    {
        return await _dbSet.AnyAsync(i =>
            i.BoardId == boardId &&
            i.InviteeEmail == email &&
            i.Status == InvitationStatus.Pending);
    }

    public async Task<BoardInvitation?> GetPendingByEmailAsync(int boardId, string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(i =>
                i.BoardId == boardId &&
                i.InviteeEmail == email &&
                i.Status == InvitationStatus.Pending);
    }

    public async Task<IEnumerable<BoardInvitation>> GetMyPendingAsync(int inviteeId)
    {
        return await _dbSet
                .Include(i => i.Board)
                .Include(i => i.Sender)
                .Where(i => i.InviteeId == inviteeId && i.Status == InvitationStatus.Pending)
                .ToListAsync();
    }
}