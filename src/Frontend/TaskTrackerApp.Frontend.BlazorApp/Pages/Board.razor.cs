using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.DTOs.Columns;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages;

public partial class Board
{
    [Parameter] public int BoardId { get; set; }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

    private BoardDto? board;
    private List<ColumnDto> columns = new();
    private bool isLoading = true;
    private Dictionary<int, List<CardDto>> cardsByColumn = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadBoardDataAsync();
    }

    private async Task LoadBoardDataAsync()
    {
        isLoading = true;
        try
        {
            var boardResult = await BoardsService.GetBoardByIdAsync(BoardId);
            if (!boardResult.IsSuccess) return;
            board = boardResult.Value;

            var columnsResult = await ColumnsService.GetByBoardIdAsync(BoardId);
            if (columnsResult.IsSuccess && columnsResult.Value != null)
            {
                columns = columnsResult.Value.ToList();

                cardsByColumn.Clear();
                foreach (var col in columns)
                {
                    var cardsResult = await CardsService.GetCardsByColumnId(col.Id);
                    if (cardsResult.IsSuccess)
                    {
                        cardsByColumn[col.Id] = cardsResult.Value.ToList();
                    }
                    else
                    {
                        cardsByColumn[col.Id] = new List<CardDto>();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to load board data.", Severity.Error);
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task HandleAddColumn()
    {
        var dialog = await DialogService.ShowAsync<CreateColumnDialog>("Add List");
        var result = await dialog.Result;

        if (!result.Canceled && result.Data is CreateColumnDto newColumnDto)
        {
            newColumnDto.BoardId = BoardId;

            var apiResult = await ColumnsService.CreateColumnAsync(newColumnDto);

            if (apiResult.IsSuccess)
            {
                await LoadBoardDataAsync();
            }
            else
            {
                Snackbar.Add(apiResult.Error.Message, Severity.Error);
            }
        }
    }

    private async Task HandleAddCard(int columnId)
    {
        var dialog = await DialogService.ShowAsync<CreateCardDialog>("Add Card");
        var result = await dialog.Result;

        if (!result.Canceled && result.Data is CreateCardDto cardData)
        {
            cardData.ColumnId = columnId;
            cardData.BoardId = BoardId;

            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    cardData.CreatedById = userId;
                }
            }

            var apiResult = await CardsService.CreateCardAsync(cardData);

            if (apiResult.IsSuccess)
            {
                await LoadBoardDataAsync();
            }
            else
            {
                Snackbar.Add(apiResult.Error.Message, Severity.Error);
            }
        }
    }

    private async Task HandleDeleteColumn(int columnId)
    {
        bool? confirm = await DialogService.ShowMessageBox(
            "Delete List",
            "Are you sure? All tasks in this list will be deleted.",
            yesText: "Delete", cancelText: "Cancel");

        if (confirm == true)
        {
            var result = await ColumnsService.DeleteColumnAsync(columnId);
            if (result.IsSuccess)
            {
                await LoadBoardDataAsync();
                Snackbar.Add("List deleted", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error.Message, Severity.Error);
            }
        }
    }

    private bool IsOverdue(CardDto card)
    {
        if (card.IsCompleted)
        {
            return false;
        }

        if (card.DueDate == null)
        {
            return false;
        }

        return card.DueDate.Value < DateTime.Now;
    }

    private void HandleCardClick(CardDto card)
    {
        Snackbar.Add($"Clicked card: {card.Title}", Severity.Info);
    }
}