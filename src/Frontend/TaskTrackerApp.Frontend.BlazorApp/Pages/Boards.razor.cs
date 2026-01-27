using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.BoardDialogs;
using TaskTrackerApp.Frontend.Domain;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.Events.BoardMember;
using TaskTrackerApp.Frontend.Domain.Events.Invitations;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Hubs;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages;

public partial class Boards : IDisposable
{
    [Inject] private IBoardsService BoardsService { get; set; } = default!;

    [Inject] private IDialogService DialogService { get; set; } = default!;

    [Inject] private ISnackbar SnackBar { get; set; } = default!;

    [Inject] private NavigationManager Nav { get; set; } = default!;

    [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    [Inject] private InvitationSignalRService InvitationHub { get; set; }

    [Inject] private BoardSignalRService BoardHub { get; set; }

    private IEnumerable<BoardDto> lastOpenedBoards = Enumerable.Empty<BoardDto>();
    private IEnumerable<BoardDto> allBoards = Enumerable.Empty<BoardDto>();
    private bool isLoading = true;

    private HashSet<int> _activeBoardIds = new();

    protected override async Task OnInitializedAsync()
    {
        InvitationHub.OnInviteResponded += HandleInviteResponded;

        BoardHub.OnMemberRemoved += HandleMemberRemoved;

        await LoadBoardsAsync();
    }

    private async Task LoadBoardsAsync()
    {
        isLoading = true;
        try
        {
            var result = await BoardsService.GetAllAsync();

            if (result.IsSuccess && result.Value is not null)
            {
                var data = result.Value.ToList();
                allBoards = data.OrderBy(x => x.Title);

                await LoadRecentBoardsFromStorage(data);
                foreach (var board in allBoards)
                {
                    if (!_activeBoardIds.Contains(board.Id))
                    {
                        await BoardHub.JoinBoard(board.Id);
                        _activeBoardIds.Add(board.Id);
                    }
                }
            }
            else
            {
                SnackBar.Add(result.Error.Message, Severity.Error);
            }
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async void HandleInviteResponded(InvitationRespondedEvent e)
    {
        if (e.IsAccepted)
        {
            await LoadBoardsAsync();
            StateHasChanged();
        }
    }

    private async void HandleMemberRemoved(BoardMemberRemovedEvent e)
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var myIdStr = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(myIdStr, out int myId) && e.UserId == myId)
        {
            allBoards = allBoards.Where(b => b.Id != e.BoardId).ToList();
            lastOpenedBoards = lastOpenedBoards.Where(b => b.Id != e.BoardId).ToList();

            _ = BoardHub.LeaveBoard(e.BoardId);
            _activeBoardIds.Remove(e.BoardId);

            SnackBar.Add("You have been removed from a board.", Severity.Warning);
            StateHasChanged();
        }
    }

    private async Task LoadRecentBoardsFromStorage(List<BoardDto> apiBoards)
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userIdStr = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr)) return;

        var key = $"recentBoardsState-{userIdStr}";
        var recentItems = await LocalStorage.GetItemAsync<List<RecentBoardItem>>(key);

        if (recentItems != null && recentItems.Any())
        {
            var tempList = new List<BoardDto>();

            foreach (var item in recentItems)
            {
                var matchingBoard = apiBoards.FirstOrDefault(b => b.Id == item.BoardId);
                if (matchingBoard != null)
                {
                    matchingBoard.LastTimeOpenned = item.LastViewed;
                    tempList.Add(matchingBoard);
                }
            }

            lastOpenedBoards = tempList
                .OrderByDescending(x => x.LastTimeOpenned)
                .Take(4);
        }
    }

    private void HandleBoardClick(int boardId)
    {
        Nav.NavigateTo($"/board/{boardId}");
    }

    private async void HandleCreateBoard()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = await DialogService.ShowAsync<CreateBoardDialog>("Create New Board", options);
        var result = await dialog.Result;

        if (!result.Canceled && result.Data is CreateBoardDto newBoardModel)
        {
            var createResult = await BoardsService.CreateAsync(newBoardModel);
            if (createResult.IsSuccess)
            {
                SnackBar.Add("Board created successfully!", Severity.Success);
                await LoadBoardsAsync();
                StateHasChanged();
            }
            else
            {
                SnackBar.Add(createResult.Error.Message, Severity.Error);
            }
        }
    }

    private async Task DeleteBoard(int boardId)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Delete Board",
            "Are you sure you want to delete this board? This cannot be undone.",
            yesText: "Delete", cancelText: "Cancel");

        if (result == true)
        {
            var deleteResult = await BoardsService.DeleteAsync(boardId);
            if (deleteResult.IsSuccess)
            {
                SnackBar.Add("Board deleted", Severity.Success);
                await RemoveFromRecentBoards(boardId);

                allBoards = allBoards.Where(b => b.Id != boardId).ToList();
                lastOpenedBoards = lastOpenedBoards.Where(b => b.Id != boardId).ToList();

                _ = BoardHub.LeaveBoard(boardId);
                _activeBoardIds.Remove(boardId);

                StateHasChanged();
            }
            else
            {
                SnackBar.Add("Failed to delete board", Severity.Error);
            }
        }
    }

    private async Task RemoveFromRecentBoards(int boardId)
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId)) return;

        var key = $"recentBoardsState-{userId}";
        var recent = await LocalStorage.GetItemAsync<List<RecentBoardItem>>(key);

        if (recent != null)
        {
            var itemToRemove = recent.FirstOrDefault(x => x.BoardId == boardId);

            if (itemToRemove != null)
            {
                recent.Remove(itemToRemove);
                await LocalStorage.SetItemAsync(key, recent);
            }
        }
    }

    private void ArchiveBoard(int boardId)
    {
        SnackBar.Add("Archived (Not implemented yet)", Severity.Info);
    }

    public void Dispose()
    {
        InvitationHub.OnInviteResponded -= HandleInviteResponded;
        BoardHub.OnMemberRemoved -= HandleMemberRemoved;

        foreach (var boardId in _activeBoardIds)
        {
            _ = BoardHub.LeaveBoard(boardId);
        }
    }
}