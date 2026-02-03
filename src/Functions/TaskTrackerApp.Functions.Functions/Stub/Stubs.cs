using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Events.BoardMember;
using TaskTrackerApp.Domain.Events.Card;
using TaskTrackerApp.Domain.Events.Column;
using TaskTrackerApp.Domain.Events.Comment;
using TaskTrackerApp.Domain.Events.Invitations;
using TaskTrackerApp.Domain.Events.Labels;

namespace TaskTrackerApp.Functions.Functions.Stub;

public class StubPasswordHasher : IPasswordHasher
{
    public string Generate(string password) => string.Empty;

    public string Hash(string password) => string.Empty;

    public bool Verify(string password, string hashedPassword) => true;
}

public class StubTokenService : ITokenService
{
    public string CreateAccessToken(User user, out DateTime expiration)
    {
        expiration = DateTime.UtcNow;
        return "dummy-access-token";
    }

    public (string RefreshToken, DateTime Expiration) CreateRefreshToken()
    {
        return ("dummy-refresh-token", DateTime.UtcNow);
    }

    public string GenerateToken(int userId, string email, string role) => "dummy-token";
}

public class StubCurrentUserService : ICurrentUserService
{
    public int UserId => 0;

    int? ICurrentUserService.UserId => UserId;
}

public class StubNotifier : IBoardNotifier, ICardNotifier, IInvitationNotifier
{
    // --- IBoardNotifier ---
    public Task NotifyBoardUpdated(int b) => Task.CompletedTask;

    public Task NotifyMemberAdded(int b, int u) => Task.CompletedTask;

    public Task NotifyMemberAddedAsync(BoardMemberAddedEvent e) => Task.CompletedTask;

    public Task NotifyMemberRemoved(int b, int u) => Task.CompletedTask;

    public Task NotifyMemberRemovedAsync(BoardMemberRemovedEvent e) => Task.CompletedTask;

    public Task NotifyMemberRoleUpdatedAsync(BoardMemberRoleUpdatedEvent e) => Task.CompletedTask;

    // --- IInvitationNotifier ---
    public Task Notify(BoardInvitationDto i) => Task.CompletedTask;

    public Task NotifyInvitationAddedAsync(BoardInvitationAddedEvent e) => Task.CompletedTask;

    public Task NotifyInvitationRevokedAsync(BoardInvitationRevokedEvent e) => Task.CompletedTask;

    public Task NotifySenderInviteRespondedAsync(int s, string i, string b, bool a) => Task.CompletedTask;

    public Task NotifyUserInviteRevokedAsync(int u, int i) => Task.CompletedTask;

    public Task NotifyUserReceivedInviteAsync(int u, int i, int b, string s, string bn) => Task.CompletedTask;

    // --- ICardNotifier ---
    public Task NotifyCardCreated(CardDto c) => Task.CompletedTask;

    public Task NotifyCardCreatedAsync(CardCreatedEvent e) => Task.CompletedTask;

    public Task NotifyCardDeleted(int c) => Task.CompletedTask;

    public Task NotifyCardDeletedAsync(CardDeletedEvent e) => Task.CompletedTask;

    public Task NotifyCardMoved(int c, int n) => Task.CompletedTask;

    public Task NotifyCardMovedAsync(CardMovedEvent e) => Task.CompletedTask;

    public Task NotifyCardUpdated(CardDto c) => Task.CompletedTask;

    public Task NotifyCardUpdatedAsync(CardUpdatedEvent e) => Task.CompletedTask;

    public Task NotifyColumnCreatedAsync(ColumnCreatedEvent e) => Task.CompletedTask;

    public Task NotifyColumnDeletedAsync(ColumnDeletedEvent e) => Task.CompletedTask;

    public Task NotifyColumnMovedAsync(ColumnMovedEvent e) => Task.CompletedTask;

    public Task NotifyCommentAddedAsync(CommentAddedEvent e) => Task.CompletedTask;

    public Task NotifyCommentDeletedAsync(int c, int cid) => Task.CompletedTask;

    public Task NotifyCommentUpdatedAsync(CommentUpdatedEvent e) => Task.CompletedTask;

    public Task NotifyLabelAddedAsync(int c, int l) => Task.CompletedTask;

    public Task NotifyLabelCreatedAsync(LabelCreatedEvent e) => Task.CompletedTask;

    public Task NotifyLabelDeletedAsync(LabelDeletedEvent e) => Task.CompletedTask;

    public Task NotifyLabelRemovedAsync(int c, int l) => Task.CompletedTask;

    public Task NotifyLabelUpdatedAsync(LabelUpdatedEvent e) => Task.CompletedTask;

    public Task NotifyUserAssignedToCardAsync(int u, int c, string t) => Task.CompletedTask;
}