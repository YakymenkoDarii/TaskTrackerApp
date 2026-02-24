using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.BoardDialogs;
using TaskTrackerApp.Frontend.Domain;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards.Requests;
using TaskTrackerApp.Frontend.Domain.Events.BoardMember;
using TaskTrackerApp.Frontend.Domain.Events.Invitations;
using TaskTrackerApp.Frontend.Domain.Results;
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

    [Inject] private ISubscriptionService SubscriptionService { get; set; }

    private List<BoardDto> StarredBoards = new();
    private List<BoardDto> OwnedBoards = new();
    private List<BoardDto> SharedBoards = new();
    private IEnumerable<BoardDto> lastOpenedBoards = Enumerable.Empty<BoardDto>();
    private List<BoardDto> allBoards = new();

    private bool isLoading = true;
    private HashSet<int> _activeBoardIds = new();
    private int _currentUserId;

    private bool HasLockedBoards => allBoards.Any(b => b.IsLocked);

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var myIdStr = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(myIdStr, out int myId))
        {
            _currentUserId = myId;
        }

        InvitationHub.OnInviteResponded += HandleInviteResponded;
        BoardHub.OnMemberRemoved += HandleMemberRemoved;

        await LoadBoardsAsync();
    }

    private async Task LoadBoardsAsync()
    {
        isLoading = true;
        try
        {
            var ownedResult = await BoardsService.GetOwnedBoardsAsync();
            var sharedResult = await BoardsService.GetSharedWithMeBoardsAsync();

            var owned = ownedResult.IsSuccess && ownedResult.Value != null ? ownedResult.Value.ToList() : new List<BoardDto>();
            var shared = sharedResult.IsSuccess && sharedResult.Value != null ? sharedResult.Value.ToList() : new List<BoardDto>();

            allBoards = owned.Concat(shared)
                             .OrderBy(x => x.IsLocked)
                             .ThenBy(x => x.Title)
                             .ToList();

            StarredBoards = allBoards.Where(b => b.IsStarred).ToList();
            OwnedBoards = owned.Where(b => !b.IsStarred).ToList();
            SharedBoards = shared.Where(b => !b.IsStarred).ToList();

            await LoadRecentBoardsFromStorage(allBoards);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task ToggleBoardStar(BoardDto board)
    {
        board.IsStarred = !board.IsStarred;

        Result result;

        if (board.IsStarred)
        {
            StarredBoards.Add(board);
            OwnedBoards.Remove(board);
            SharedBoards.Remove(board);

            var request = new UpdateStarRequest { IsStarred = board.IsStarred };
            result = await BoardsService.UpdateBoardStarAsync(board.Id, request);
        }
        else
        {
            var request = new UpdateStarRequest { IsStarred = board.IsStarred };
            result = await BoardsService.UpdateBoardStarAsync(board.Id, request);

            StarredBoards.Remove(board);
            await LoadBoardsAsync();
        }
        StateHasChanged();

        if (!result.IsSuccess)
        {
            SnackBar.Add("Failed to update board star.", Severity.Error);
            await LoadBoardsAsync();
        }
    }

    private string GetBoardCardClass(bool isLocked)
    {
        if (isLocked)
        {
            return "cursor-pointer hover-effect mud-theme-dark opacity-80 border-solid border-2 mud-border-error";
        }

        return "cursor-pointer hover-effect height-100";
    }

    private async Task NavigateToUpgrade()
    {
        var result = await SubscriptionService.CreateCheckoutSessionAsync();
        if (result.IsSuccess)
        {
            Nav.NavigateTo(result.Value!, forceLoad: true);
        }
        else
        {
            SnackBar.Add("Could not initiate upgrade.", Severity.Error);
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
                    matchingBoard.LastModified = item.LastViewed;
                    tempList.Add(matchingBoard);
                }
            }
            lastOpenedBoards = tempList.OrderByDescending(x => x.LastModified).Take(4);
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

    public void Dispose()
    {
        InvitationHub.OnInviteResponded -= HandleInviteResponded;
        BoardHub.OnMemberRemoved -= HandleMemberRemoved;
    }
}