using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.Enums;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.CardDialogs;

public partial class CardDetailsDialog
{
    [Inject] private ICardsService CardsService { get; set; }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;

    [Inject] private IBoardMembersService BoardMembersService { get; set; }

    [Parameter] public CardDto Card { get; set; } = default!;

    [Parameter] public int BoardId { get; set; }

    [Parameter] public bool IsReadOnly { get; set; }

    private string title = string.Empty;
    private string description = string.Empty;
    private DateTime? dueDate;
    private bool isCompleted;
    private CardPriority _priority = CardPriority.Low;

    private int? _assigneeId;
    private List<BoardMemberDto> _boardMembers = new();

    protected override async Task OnInitializedAsync()
    {
        if (Card != null)
        {
            title = Card.Title;
            description = Card.Description;
            dueDate = Card.DueDate;
            isCompleted = Card.IsCompleted;
            _assigneeId = Card.AssigneeId;
            _priority = Card.Priority;
        }

        await LoadBoardMembers();
    }

    private async Task LoadBoardMembers()
    {
        var result = await BoardMembersService.GetMembersAsync(BoardId);
        if (result.IsSuccess)
        {
            _boardMembers = result.Value.ToList();
            StateHasChanged();
        }
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task SaveChanges()
    {
        if (IsReadOnly) return;

        if (string.IsNullOrWhiteSpace(title))
        {
            Snackbar.Add("Title is required", Severity.Warning);
            return;
        }

        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userIdStr = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdStr, out int userId))
        {
            Snackbar.Add("User not authenticated", Severity.Error);
            return;
        }

        var updateDto = new UpdateCardDto
        {
            Id = Card.Id,
            Title = title,
            Description = description,
            DueDate = dueDate,
            ColumnId = Card.ColumnId,
            BoardId = BoardId,
            IsCompleted = isCompleted,
            UpdatedById = userId,
            AssigneeId = _assigneeId,
            Position = Card.Position,
            Priority = _priority,
        };

        var result = await CardsService.UpdateAsync(Card.Id, updateDto);

        if (result.IsSuccess)
        {
            Snackbar.Add("Card updated", Severity.Success);
            MudDialog.Close(DialogResult.Ok(result.Value));
        }
        else
        {
            Snackbar.Add(result.Error.Code, Severity.Error);
        }
    }

    private async Task DeleteCard()
    {
        if (IsReadOnly) return;

        bool? result = await DialogService.ShowMessageBox(
            "Delete Card",
            "Are you sure you want to delete this card? This cannot be undone.",
            yesText: "Delete", cancelText: "Cancel");

        if (result != true)
            return;

        var deleteResult = await CardsService.DeleteCardAsync(Card.Id);

        if (deleteResult.IsSuccess)
        {
            Snackbar.Add("Card deleted", Severity.Success);
            MudDialog.Close(DialogResult.Ok("Deleted"));
        }
        else
        {
            Snackbar.Add(deleteResult.Error.Code, Severity.Error);
        }
    }

    private async Task OnCompletionToggled(bool newValue)
    {
        isCompleted = newValue;

        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userIdStr = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(userIdStr, out int userId);

        var updateDto = new UpdateCardDto
        {
            Id = Card.Id,
            Title = title,
            Description = description,
            DueDate = dueDate,
            ColumnId = Card.ColumnId,
            BoardId = BoardId,
            IsCompleted = isCompleted,
            UpdatedById = userId,
            AssigneeId = _assigneeId,
            Position = Card.Position,
            Priority = _priority,
        };

        var result = await CardsService.UpdateAsync(Card.Id, updateDto);

        if (result.IsSuccess)
        {
            Snackbar.Add(isCompleted ? "Marked as complete" : "Marked as incomplete", Severity.Success);
            Card.IsCompleted = isCompleted;
        }
        else
        {
            isCompleted = !newValue;
            Snackbar.Add("Failed to update status", Severity.Error);
        }
    }

    private Color GetPriorityColor(CardPriority p) => p switch
    {
        CardPriority.Critical => Color.Error,
        CardPriority.High => Color.Warning,
        CardPriority.Medium => Color.Info,
        _ => Color.Default
    };
}