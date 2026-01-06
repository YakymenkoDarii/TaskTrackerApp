using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.CardDialogs;

public partial class CreateCardDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;

    private CreateCardDto model = new();

    private DateTime? date;

    private void Submit()
    {
        model.DueDate = date;

        if (string.IsNullOrWhiteSpace(model.Title))
        {
            return;
        }

        MudDialog.Close(DialogResult.Ok(model));
    }

    private void Cancel() => MudDialog.Cancel();
}