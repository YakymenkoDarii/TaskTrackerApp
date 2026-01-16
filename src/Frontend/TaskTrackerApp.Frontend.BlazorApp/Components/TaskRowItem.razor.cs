using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.Enums;

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

    private Color GetPriorityColor(CardPriority priority)
    {
        return priority switch
        {
            CardPriority.Critical => Color.Error,
            CardPriority.High => Color.Warning,
            CardPriority.Medium => Color.Info,
            _ => Color.Default
        };
    }
}