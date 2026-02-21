using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;

namespace TaskTrackerApp.Frontend.BlazorApp.Components.Cards;

public partial class CreateCardInline
{
    [Parameter] public EventCallback<CreateCardDto> OnSubmit { get; set; }

    [Parameter] public EventCallback OnCancel { get; set; }

    private CreateCardDto model = new();
    private DateTime? date;
    private TimeSpan? time;

    private async Task Submit()
    {
        if (string.IsNullOrWhiteSpace(model.Title))
            return;

        if (date.HasValue)
        {
            model.DueDate = date.Value.Date + (time ?? TimeSpan.Zero);
        }
        else
        {
            model.DueDate = null;
        }

        await OnSubmit.InvokeAsync(model);

        model = new();
        date = null;
        time = null;
    }

    private async Task Cancel()
    {
        model = new();
        date = null;
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