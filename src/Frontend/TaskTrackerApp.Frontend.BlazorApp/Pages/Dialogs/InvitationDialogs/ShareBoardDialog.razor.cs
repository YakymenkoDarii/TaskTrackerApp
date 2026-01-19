using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Domain.Events.Invitations;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Frontend.Domain.DTOs.Users;
using TaskTrackerApp.Frontend.Domain.Enums;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Hubs;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.InvitationDialogs;

public partial class ShareBoardDialog : IDisposable
{
    [Inject] public IUsersService UsersService { get; set; }

    [Inject] public IBoardMembersService MembersService { get; set; }

    [Inject] public IBoardInvitationsService InvitationsService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject] private NavigationManager Navigation { get; set; }

    [Inject] private InvitationSignalRService SignalRService { get; set; }

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    [Parameter] public int BoardId { get; set; }

    private BoardRole SelectedRole { get; set; } = BoardRole.Member;

    private List<BoardMemberDto> Members { get; set; } = new();

    private List<BoardInvitationDto> PendingInvites { get; set; } = new();

    private UserSummaryDto? SelectedUser { get; set; }

    private int? _currentUserId = null;
    private bool _isCurrentUserAdmin = false;

    protected override async Task OnInitializedAsync()
    {
        await GetCurrentUserIdentity();
        SignalRService.OnInviteResponded += HandleInviteResponded;
        await LoadData();
    }

    private async Task GetCurrentUserIdentity()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        var userIdString = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        if (!string.IsNullOrEmpty(userIdString))
        {
            if (int.TryParse(userIdString, out int parsedId))
            {
                _currentUserId = parsedId;
            }
        }
        else
        {
            _currentUserId = null;
        }
    }

    private async void HandleInviteResponded(InvitationRespondedEvent notification)
    {
        await LoadData();
        Snackbar.Add(notification.Message, notification.IsAccepted ? Severity.Success : Severity.Warning);

        StateHasChanged();
    }

    private async Task LoadData()
    {
        var membersResult = await MembersService.GetMembersAsync(BoardId);
        if (membersResult.IsSuccess)
        {
            Members = membersResult.Value.ToList();

            var myMembership = Members.FirstOrDefault(m => m.UserId == _currentUserId);
            _isCurrentUserAdmin = myMembership?.Role == BoardRole.Admin.ToString();
        }

        var invitesResult = await InvitationsService.GetPendingInvitesAsync(BoardId);
        if (invitesResult.IsSuccess)
        {
            PendingInvites = invitesResult.Value.ToList();
        }
    }

    private async Task<IEnumerable<UserSummaryDto>> SearchUsers(string value, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Enumerable.Empty<UserSummaryDto>();
        }

        var result = await UsersService.SearchUsersAsync(value, token, BoardId);

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return Enumerable.Empty<UserSummaryDto>();
    }

    private async Task SendInvite()
    {
        if (SelectedUser == null) return;

        var command = new SendBoardInvitationRequestDto
        {
            BoardId = BoardId,
            InviteeEmail = SelectedUser.Email,
            Role = SelectedRole
        };

        var result = await InvitationsService.SendInviteAsync(command);

        if (result.IsSuccess)
        {
            Snackbar.Add($"Invited {SelectedUser.Email}", Severity.Success);

            SelectedUser = null;
            SelectedRole = BoardRole.Member;

            await LoadData();
        }
        else
        {
            Snackbar.Add(result.Error.ToString() ?? "Failed to send invite", Severity.Error);
        }
    }

    private async Task ChangeMemberRole(BoardMemberDto member, BoardRole newRole)
    {
        if (!_isCurrentUserAdmin) return;

        if (member.Role == BoardRole.Admin.ToString() && newRole != BoardRole.Admin)
        {
            var adminCount = Members.Count(m => m.Role == BoardRole.Admin.ToString());
            if (adminCount <= 1)
            {
                Snackbar.Add("There must be at least one Admin on the board.", Severity.Error);
                return;
            }
        }

        var result = await MembersService.UpdateMemberRoleAsync(BoardId, member.UserId, newRole);
        if (result.IsSuccess)
        {
            Snackbar.Add("Role updated", Severity.Success);
            await LoadData();
        }
        else
        {
            Snackbar.Add(result.Error.ToString(), Severity.Error);
        }
    }

    private async Task HandleRemoveMember(BoardMemberDto member)
    {
        bool isMe = member.UserId == _currentUserId;
        bool isLastMember = Members.Count == 1;

        string title;
        string message;
        string buttonText;

        if (isMe)
        {
            if (isLastMember)
            {
                title = "Delete Board?";
                message = "You are the last member. If you leave, this board will be deleted.";
                buttonText = "Delete Board";
            }
            else
            {
                if (member.Role == BoardRole.Admin.ToString())
                {
                    var otherAdmins = Members.Any(m => m.UserId != _currentUserId && m.Role == BoardRole.Admin.ToString());
                    if (!otherAdmins)
                    {
                        Snackbar.Add("You must promote another member to Admin before leaving.", Severity.Warning);
                        return;
                    }
                }
                title = "Leave Board?";
                message = "Are you sure you want to leave this board?";
                buttonText = "Leave";
            }
        }
        else
        {
            title = "Remove User?";
            message = $"Are you sure you want to remove {member.Name}?";
            buttonText = "Remove";
        }

        bool? result = await DialogService.ShowMessageBox(
            title,
            message,
            yesText: buttonText,
            cancelText: "Cancel");

        if (result == true)
        {
            await ExecuteRemoval(member.UserId);
        }
    }

    private async Task ExecuteRemoval(int userId)
    {
        var result = await MembersService.RemoveMemberAsync(BoardId, userId);
        if (result.IsSuccess)
        {
            if (userId == _currentUserId)
            {
                MudDialog.Close(DialogResult.Ok(true));
                Navigation.NavigateTo("/");
            }
            else
            {
                Snackbar.Add("User removed", Severity.Success);
                await LoadData();
            }
        }
        else
        {
            Snackbar.Add(result.Error.ToString(), Severity.Error);
        }
    }

    private async Task RevokeInvite(int invitationId)
    {
        var result = await InvitationsService.RevokeInviteAsync(invitationId);

        if (result.IsSuccess)
        {
            Snackbar.Add("Invitation revoked", Severity.Info);
            await LoadData();
        }
        else
        {
            Snackbar.Add(result.Error.ToString(), Severity.Error);
        }
    }

    private void Cancel() => MudDialog.Cancel();

    public void Dispose()
    {
        SignalRService.OnInviteResponded -= HandleInviteResponded;
    }

    private string GetRoleDisplayName(BoardRole role) => role switch
    {
        BoardRole.Admin => "Administrator",
        BoardRole.Viewer => "Read Only",
        _ => role.ToString()
    };

    private BoardRole ParseRole(string roleString)
    {
        return Enum.TryParse<BoardRole>(roleString, true, out var r) ? r : BoardRole.Member;
    }

    private bool IsMe(int userId)
    {
        return userId == _currentUserId;
    }
}