using Microsoft.AspNetCore.Components;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.Enums;

namespace TaskTrackerApp.Frontend.BlazorApp.Components.BoardCards;

public partial class BoardCard
{
    [Parameter, EditorRequired] public BoardDto Board { get; set; } = default!;

    [Parameter, EditorRequired] public int CurrentUserId { get; set; }

    [Parameter] public bool IsLastOpenedView { get; set; } = false;

    [Parameter] public EventCallback<int> OnBoardClick { get; set; }

    [Parameter] public EventCallback<BoardDto> OnToggleStar { get; set; }

    [Parameter] public EventCallback<BoardThemeColor> OnThemeChange { get; set; }
}