using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Columns;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.ColumnDialogs;

public partial class CreateColumnDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;

    private CreateColumnDto model = new();

    private void Submit()
    {
        if (string.IsNullOrWhiteSpace(model.Title))
            return;

        MudDialog.Close(DialogResult.Ok(model));
    }

    private void Cancel() => MudDialog.Cancel();
}