using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.InvitationDialogs;

public partial class InvitationsDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    [Inject] private IBoardInvitationsService InvitationService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    private List<MyInvitationDto> _invitations = new();
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
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
}