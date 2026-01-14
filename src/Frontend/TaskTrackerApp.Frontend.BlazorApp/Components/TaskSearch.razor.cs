using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.CardDialogs;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Components;

public partial class TaskSearch
{
    [Inject] private ICardsService CardsService { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    [Parameter] public int? BoardId { get; set; }

    [Parameter] public int? AssigneeId { get; set; }

    [Parameter] public string Placeholder { get; set; } = "Search tasks...";

    [Parameter] public EventCallback OnTaskChanged { get; set; }

    private CardDto selectedCard;

    private async Task<IEnumerable<CardDto>> SearchTasks(string value, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new List<CardDto>();

        var result = await CardsService.SearchCardsAsync(value, BoardId, AssigneeId);

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return new List<CardDto>();
    }

    private async Task OnCardSelected(CardDto card)
    {
        if (card == null) return;

        selectedCard = null;

        await OpenCardDetails(card);
    }

    private async Task OpenCardDetails(CardDto card)
    {
        var parameters = new DialogParameters<CardDetailsDialog>
        {
            { x => x.Card, card },
            { x => x.BoardId, card.BoardId }
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            NoHeader = true
        };

        var dialog = await DialogService.ShowAsync<CardDetailsDialog>("Card Details", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled && OnTaskChanged.HasDelegate)
        {
            await OnTaskChanged.InvokeAsync();
        }
    }
}