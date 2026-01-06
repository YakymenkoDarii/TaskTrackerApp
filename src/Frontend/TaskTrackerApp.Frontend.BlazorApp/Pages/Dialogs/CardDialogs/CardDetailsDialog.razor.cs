using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Cards;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.CardDialogs;

public partial class CardDetailsDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] public CardDto Card { get; set; } = default!;

    [Parameter] public int BoardId { get; set; }

    [Inject] private ICardsService CardsService { get; set; }
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }

    private string title = string.Empty;
    private string description = string.Empty;
    private DateTime? dueDate;

    protected override void OnInitialized()
    {
        if (Card != null)
        {
            title = Card.Title;
            description = Card.Description;
            dueDate = Card.DueDate;
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
            UpdatedById = userId,
            AssigneeId = Card.AssigneeId
        };

        // 3. Call Service
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
}