using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Frontend.Domain.DTOs.Users;
using TaskTrackerApp.Frontend.Domain.Enums;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.InvitationDialogs;

public partial class ShareBoardDialog
{
    [Inject] public IUsersService UsersService { get; set; }

    [Inject] public IBoardMembersService MembersService { get; set; }

    [Inject] public IBoardInvitationsService InvitationsService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    [Parameter] public int BoardId { get; set; }

    private BoardRole SelectedRole { get; set; } = BoardRole.Member;

    private List<BoardMemberDto> Members { get; set; } = new();

    private List<BoardInvitationDto> PendingInvites { get; set; } = new();

    private UserSummaryDto? SelectedUser { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        var membersResult = await MembersService.GetMembersAsync(BoardId);
        if (membersResult.IsSuccess)
        {
            Members = membersResult.Value.ToList();
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

    private string GetRoleDisplayName(BoardRole role) => role switch
    {
        BoardRole.Admin => "Administrator",
        BoardRole.Viewer => "Read Only",
        _ => role.ToString()
    };
}