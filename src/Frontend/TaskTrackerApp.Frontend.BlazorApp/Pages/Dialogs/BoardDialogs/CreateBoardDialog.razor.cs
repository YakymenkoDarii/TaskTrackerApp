using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.BoardDialogs;

public partial class CreateBoardDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;

    private CreateBoardDto model = new CreateBoardDto();

    private void Cancel() => MudDialog.Cancel();

    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" || e.Key == "NumpadEnter")
        {
            Submit();
        }
    }

    private void Submit()
    {
        if (string.IsNullOrWhiteSpace(model.Title))
        {
            return;
        }

        MudDialog.Close(DialogResult.Ok(model));
    }
}