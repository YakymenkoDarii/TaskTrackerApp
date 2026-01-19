using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Domain.Events.Invitations;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Hubs;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.InvitationDialogs;

public partial class InvitationsDialog : IDisposable
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    [Inject] private IBoardInvitationsService InvitationService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private InvitationSignalRService SignalRService { get; set; }

    private List<MyInvitationDto> _invitations = new();
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        SignalRService.OnInviteReceived += HandleInviteReceived;
        SignalRService.OnInviteRevoked += HandleInviteRevoked;

        await LoadInvitations();
    }

    private async Task LoadInvitations()
    {
        _isLoading = true;
        var result = await InvitationService.GetMyPendingInvitations();

        if (result.IsSuccess)
        {
            _invitations = result.Value.ToList();
        }
        _isLoading = false;
    }

    private void HandleInviteReceived(InvitationReceivedEvent notification)
    {
        var newInvite = new MyInvitationDto
        {
            Id = notification.InvitationId,
            BoardId = notification.BoardId,
            SenderName = notification.Sender,
            BoardTitle = notification.BoardName,
            SenderAvatarUrl = null
        };

        _invitations.Insert(0, newInvite);

        Snackbar.Add($"Invited to {notification.BoardName} by {notification.Sender}", Severity.Info);

        StateHasChanged();
    }

    private void HandleInviteRevoked(int invitationId)
    {
        var invite = _invitations.FirstOrDefault(i => i.Id == invitationId);
        if (invite != null)
        {
            _invitations.Remove(invite);
            Snackbar.Add("An invitation was revoked.", Severity.Warning);
            StateHasChanged();
        }
    }

    private async Task Respond(int invitationId, bool isAccepted)
    {
        var request = new RespondToInvitationRequestDto
        {
            InvitationId = invitationId,
            IsAccepted = isAccepted
        };

        var result = await InvitationService.RespondToInvite(request);

        if (result.IsSuccess)
        {
            Snackbar.Add(isAccepted ? "Joined board successfully!" : "Invitation declined.", Severity.Success);

            var item = _invitations.FirstOrDefault(i => i.Id == invitationId);
            if (item != null)
            {
                _invitations.Remove(item);
            }
        }
        else
        {
            Snackbar.Add(result.Error?.Message ?? "Failed to respond.", Severity.Error);
        }
    }

    private void Cancel() => MudDialog.Cancel();

    public void Dispose()
    {
        SignalRService.OnInviteReceived -= HandleInviteReceived;
        SignalRService.OnInviteRevoked -= HandleInviteRevoked;
    }
}