using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.CardDialogs;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.ColumnDialogs;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.DTOs.Columns;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages;

public partial class Board
{
    [Parameter] public int BoardId { get; set; }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject] private IBoardsService BoardsService { get; set; }

    [Inject] private IColumnsService ColumnsService { get; set; }

    [Inject] private ICardsService CardsService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    private BoardDto? board;
    private List<ColumnDto> columns = new();
    private bool isLoading = true;
    private Dictionary<int, List<CardDto>> cardsByColumn = new();
    private MudDropContainer<ColumnDto> _dropContainer;

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

    private async Task ColumnDropped(MudItemDropInfo<ColumnDto> dropItem)
    {
        columns.Remove(dropItem.Item);
        columns.Insert(dropItem.IndexInZone, dropItem.Item);

        StateHasChanged();

        int userId = 0;
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity is not null && user.Identity.IsAuthenticated)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null) int.TryParse(userIdClaim.Value, out userId);
        }

        var updateDto = new UpdateColumnDto
        {
            Id = dropItem.Item.Id,
            Title = dropItem.Item.Title,
            Description = dropItem.Item.Description,
            BoardId = BoardId,
            UpdatedById = userId,
            Position = dropItem.IndexInZone
        };

        var result = await ColumnsService.UpdateColumnAsync(dropItem.Item.Id, updateDto);

        if (!result.IsSuccess)
        {
            Snackbar.Add("Failed to save column order", Severity.Error);
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
                var nextPosition = columns.Any() ? columns.Max(c => c.Position) + 1 : 0;

                var newColumn = new ColumnDto
                {
                    Id = apiResult.Value,
                    Title = newColumnDto.Title,
                    Description = newColumnDto.Description,
                    Position = nextPosition
                };

                columns.Add(newColumn);
                cardsByColumn[newColumn.Id] = new List<CardDto>();

                _dropContainer.Refresh();
                StateHasChanged();
            }
            else
            {
                Snackbar.Add(apiResult.Error.Message, Severity.Error);
            }
        }
    }

    private async Task HandleAddCard(int columnId)
    {
        var dialog = await DialogService.ShowAsync<CreateColumnDialog>("Add List");
        var result = await dialog.Result;

        if (!result.Canceled && result.Data is CreateColumnDto newColumnDto)
        {
            newColumnDto.BoardId = BoardId;

            var apiResult = await ColumnsService.CreateColumnAsync(newColumnDto);

            if (apiResult.IsSuccess)
            {
                var nextPosition = columns.Any() ? columns.Max(c => c.Position) + 1 : 0;

                var newColumn = new ColumnDto
                {
                    Id = apiResult.Value,
                    Title = newColumnDto.Title,
                    Description = newColumnDto.Description,
                    Position = nextPosition
                };

                columns.Add(newColumn);
                cardsByColumn[newColumn.Id] = new List<CardDto>();

                StateHasChanged();
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
                var columnToRemove = columns.FirstOrDefault(c => c.Id == columnId);
                if (columnToRemove != null)
                {
                    columns.Remove(columnToRemove);
                    cardsByColumn.Remove(columnId);

                    if (_dropContainer != null)
                    {
                        _dropContainer.Refresh();
                    }

                    StateHasChanged();
                }
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

    private async Task HandleCardClick(CardDto card)
    {
        Console.WriteLine("THE CARD IS CLICKED, YOU JUST STUPID");
        var parameters = new DialogParameters<CardDetailsDialog>
        {
            { x => x.Card, card },
            { x => x.BoardId, BoardId }
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

        if (!result.Canceled && result.Data is CardDto updatedCard)
        {
            if (cardsByColumn.TryGetValue(card.ColumnId, out var oldList))
            {
                var existingItem = oldList.FirstOrDefault(c => c.Id == card.Id);
                if (existingItem != null) oldList.Remove(existingItem);
            }

            if (cardsByColumn.TryGetValue(updatedCard.ColumnId, out var newList))
            {
                newList.Add(updatedCard);
            }
            else
            {
                cardsByColumn[updatedCard.ColumnId] = new List<CardDto> { updatedCard };
            }

            StateHasChanged();
        }
    }
}