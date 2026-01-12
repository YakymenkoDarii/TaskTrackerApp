using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
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

    private string title = string.Empty;
    private string description = string.Empty;
    private DateTime? dueDate;
    private bool isCompleted;

    private int? _assigneeId;
    private List<BoardMemberDto> _boardMembers = new();

    protected override async void OnInitialized()
    {
        if (Card != null)
        {
            title = Card.Title;
            description = Card.Description;
            dueDate = Card.DueDate;
            isCompleted = Card.IsCompleted;

            _assigneeId = Card.AssigneeId;
        }

        await LoadBoardMembers();
    }

    private async Task LoadBoardMembers()
    {
        var result = await BoardMembersService.GetMembersAsync(BoardId);
        if (result.IsSuccess)
        {
            _boardMembers = result.Value.ToList();
        }
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task SaveChanges()
    {
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
}