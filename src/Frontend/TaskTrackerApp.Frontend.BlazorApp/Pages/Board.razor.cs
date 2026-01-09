using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.CardDialogs;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.ColumnDialogs;
using TaskTrackerApp.Frontend.Domain;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.DTOs.Columns;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Boards;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages;

[Authorize]
public partial class Board
{
    [Parameter] public int BoardId { get; set; }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject] private IBoardsService BoardsService { get; set; }

    [Inject] private IColumnsService ColumnsService { get; set; }

    [Inject] private ICardsService CardsService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    [Inject] private ILocalStorageService LocalStorage { get; set; }

    private BoardDto? board;

    private List<ColumnDto> columns = new();

    private List<CardDto> _allCards = new();

    private bool isLoading = true;

    private bool _isLayoutMode = false;

    private MudDropContainer<ColumnDto> _columnDropContainer;

    private MudDropContainer<CardDto> _cardDropContainer;

    protected override async Task OnInitializedAsync()
    {
        await AddToRecentBoardsAsync(BoardId);

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
                columns = columnsResult.Value.OrderBy(c => c.Position).ToList();

                _allCards.Clear();
                foreach (var col in columns)
                {
                    var cardsResult = await CardsService.GetCardsByColumnId(col.Id);
                    if (cardsResult.IsSuccess && cardsResult.Value != null)
                    {
                        _allCards.AddRange(cardsResult.Value.OrderBy(c => c.Position));
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

    private async Task CardDropped(MudItemDropInfo<CardDto> dropItem)
    {
        dropItem.Item.ColumnId = int.Parse(dropItem.DropzoneIdentifier);

        int userId = 0;
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity is not null && user.Identity.IsAuthenticated)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null) int.TryParse(userIdClaim.Value, out userId);
        }

        var updateDto = new UpdateCardDto
        {
            Id = dropItem.Item.Id,
            Title = dropItem.Item.Title,
            Description = dropItem.Item.Description,
            ColumnId = int.Parse(dropItem.DropzoneIdentifier),
            DueDate = dropItem.Item.DueDate,
            IsCompleted = dropItem.Item.IsCompleted,
            BoardId = BoardId,
            UpdatedById = userId,
            Position = dropItem.IndexInZone
        };

        var result = await CardsService.UpdateAsync(dropItem.Item.Id, updateDto);

        if (!result.IsSuccess)
        {
            Snackbar.Add("Failed to move card", Severity.Error);
            await LoadBoardDataAsync();
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
                if (_columnDropContainer != null) _columnDropContainer.Refresh();
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
        var dialog = await DialogService.ShowAsync<CreateColumnDialog>("Add Card");
        var result = await dialog.Result;

        if (!result.Canceled && result.Data is CreateColumnDto newCardInput)
        {
            var newCardDto = new CreateCardDto
            {
                Title = newCardInput.Title,
                Description = newCardInput.Description,
                ColumnId = columnId,
                BoardId = BoardId
            };

            var apiResult = await CardsService.CreateCardAsync(newCardDto);

            if (apiResult.IsSuccess)
            {
                var nextPosition = _allCards.Any(c => c.ColumnId == columnId)
                    ? _allCards.Where(c => c.ColumnId == columnId).Max(c => c.Position) + 1
                    : 0;

                var newCard = new CardDto
                {
                    Id = apiResult.Value,
                    Title = newCardInput.Title,
                    Description = newCardInput.Description,
                    ColumnId = columnId,
                    Position = nextPosition,
                };

                _allCards.Add(newCard);

                if (_cardDropContainer != null) _cardDropContainer.Refresh();
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
                    _allCards.RemoveAll(c => c.ColumnId == columnId);
                    StateHasChanged();
                }
            }
        }
    }

    private bool IsOverdue(CardDto card)
    {
        if (card.IsCompleted) return false;
        if (card.DueDate == null) return false;
        return card.DueDate.Value < DateTime.Now;
    }

    private async Task HandleCardClick(CardDto card)
    {
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

        if (!result.Canceled)
        {
            if (result.Data is CardDto updatedCard)
            {
                var index = _allCards.FindIndex(c => c.Id == updatedCard.Id);
                if (index != -1) _allCards[index] = updatedCard;
                else _allCards.Add(updatedCard);

                _allCards = _allCards.OrderBy(c => c.Position).ToList();
            }
            else if (result.Data is string action && action == "Deleted")
            {
                var itemToRemove = _allCards.FirstOrDefault(c => c.Id == card.Id);
                if (itemToRemove != null)
                {
                    _allCards.Remove(itemToRemove);
                }
            }

            if (_cardDropContainer != null) _cardDropContainer.Refresh();
            StateHasChanged();
        }
    }

    private async Task HandleToggleComplete(CardDto card)
    {
        card.IsCompleted = !card.IsCompleted;

        int userId = await GetUserId();

        var updateCard = new UpdateCardDto
        {
            Id = card.Id,
            Title = card.Title,
            Description = card.Description,
            DueDate = card.DueDate,
            ColumnId = card.ColumnId,
            BoardId = BoardId,
            AssigneeId = card.AssigneeId,
            UpdatedById = userId,
            IsCompleted = card.IsCompleted,
            Position = card.Position,
        };

        await CardsService.UpdateAsync(card.Id, updateCard);

        _cardDropContainer.Refresh();
        StateHasChanged();
    }

    private async Task SaveColumn(ColumnDto column)
    {
        int userId = await GetUserId();
        var updateDto = new UpdateColumnDto
        {
            Id = column.Id,
            Title = column.Title,
            Description = column.Description,
            BoardId = BoardId,
            Position = column.Position,
            UpdatedById = userId,
        };

        await ColumnsService.UpdateColumnAsync(column.Id, updateDto);
    }

    private async Task UpdateBoardDetails(string val)
    {
        int userId = await GetUserId();

        var updateDto = new UpdateBoardDto
        {
            Id = board.Id,
            Title = board.Title,
            Description = board.Description,
            UpdatedById = userId,
        };

        await BoardsService.UpdateAsync(board.Id, updateDto);
    }

    private async Task<int> GetUserId()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        int userId = 0;

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null)
        {
            int.TryParse(userIdClaim.Value, out userId);
        }

        return userId;
    }

    private async Task AddToRecentBoardsAsync(int boardId)
    {
        var userId = await GetUserId();
        var key = $"recentBoardsState-{userId}";

        var recent = await LocalStorage.GetItemAsync<List<RecentBoardItem>>(key) ?? new();

        var existing = recent.FirstOrDefault(x => x.BoardId == boardId);
        if (existing != null) recent.Remove(existing);

        recent.Insert(0, new RecentBoardItem { BoardId = boardId, LastViewed = DateTime.UtcNow });

        if (recent.Count > 5) recent = recent.Take(5).ToList();

        await LocalStorage.SetItemAsync(key, recent);
    }
}