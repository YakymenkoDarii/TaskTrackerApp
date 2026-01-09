using Microsoft.AspNetCore.Components;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;

namespace TaskTrackerApp.Frontend.BlazorApp.Components;

public partial class TaskRowItem
{
    [Parameter] public UpcomingCardDto Task { get; set; } = default!;

    [Parameter] public EventCallback<UpcomingCardDto> OnTaskClick { get; set; }

    [Parameter] public EventCallback<UpcomingCardDto> OnToggleComplete { get; set; }

    private bool IsOverdue(UpcomingCardDto task)
    {
        return !task.IsCompleted && task.DueDate < DateTime.Now;
    }
}