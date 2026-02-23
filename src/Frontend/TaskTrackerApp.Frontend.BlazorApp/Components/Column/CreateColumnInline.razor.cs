using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TaskTrackerApp.Frontend.Domain.DTOs.Columns;

namespace TaskTrackerApp.Frontend.BlazorApp.Components.Column;

public partial class CreateColumnInline
{
    [Parameter] public EventCallback<CreateColumnDto> OnSubmit { get; set; }

    [Parameter] public EventCallback OnCancel { get; set; }

    private CreateColumnDto model = new();

    private async Task Submit()
    {
        if (string.IsNullOrWhiteSpace(model.Title))
            return;

        await OnSubmit.InvokeAsync(model);
        model = new();
    }

    private async Task Cancel()
    {
        model = new();
        await OnCancel.InvokeAsync();
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await Submit();
        }
        else if (e.Key == "Escape")
        {
            await Cancel();
        }
    }
}