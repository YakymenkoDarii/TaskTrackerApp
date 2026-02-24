using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.BlazorApp.Layout;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.BoardDialogs;
using TaskTrackerApp.Frontend.Domain;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards.Requests;
using TaskTrackerApp.Frontend.Domain.Enums;
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

    [Inject] private ISubscriptionService SubscriptionService { get; set; }

    [CascadingParameter] public MainLayout? MainLayout { get; set; }

    private List<BoardDto> StarredBoards = new();
    private List<BoardDto> OwnedBoards = new();
    private List<BoardDto> SharedBoards = new();
    private IEnumerable<BoardDto> lastOpenedBoards = Enumerable.Empty<BoardDto>();

    private List<BoardDto> allBoards = new();
    private List<BoardDto> rawLastOpenedBoards = new();

    private string _searchString = string.Empty;
    private string _sortOption = "Last Opened";
    private bool isLoading = true;
    private HashSet<int> _activeBoardIds = new();
    private int _currentUserId;

    private int _ownedCount = 0;
    private readonly int _maxFreeBoards = 3;
    private bool _isPro => MainLayout?.IsUserPro ?? false;

    private bool IsMaxedOut => !_isPro && _ownedCount >= _maxFreeBoards;

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

            allBoards = owned.Concat(shared).ToList();

            await LoadRecentBoardsFromStorage(allBoards);

            ApplyFilterAndSort();
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private void OnSearchChanged(string text)
    {
        _searchString = text;
        ApplyFilterAndSort();
    }

    private void OnSortChanged(string option)
    {
        _sortOption = option;
        ApplyFilterAndSort();
    }

    private void ApplyFilterAndSort()
    {
        var filtered = allBoards.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            filtered = filtered.Where(b => b.Title.Contains(_searchString, StringComparison.OrdinalIgnoreCase));
        }

        filtered = _sortOption switch
        {
            "Alphabetical" => filtered.OrderBy(b => b.Title),
            "Newest Created" => filtered.OrderByDescending(b => b.Id),
            _ => filtered.OrderByDescending(b => b.LastModified)
        };

        var finalBoards = filtered.ToList();

        StarredBoards = finalBoards.Where(b => b.IsStarred).ToList();
        OwnedBoards = finalBoards.Where(b => !b.IsStarred && b.CreatedById == _currentUserId).ToList();
        SharedBoards = finalBoards.Where(b => !b.IsStarred && b.CreatedById != _currentUserId).ToList();

        _ownedCount = finalBoards.Count(b => b.CreatedById == _currentUserId);

        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            lastOpenedBoards = rawLastOpenedBoards.Where(b => b.Title.Contains(_searchString, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            lastOpenedBoards = rawLastOpenedBoards;
        }
    }

    private async Task ToggleBoardStar(BoardDto board)
    {
        board.IsStarred = !board.IsStarred;
        ApplyFilterAndSort();

        var request = new UpdateStarRequest { IsStarred = board.IsStarred };
        var result = await BoardsService.UpdateBoardStarAsync(board.Id, request);

        if (!result.IsSuccess)
        {
            SnackBar.Add("Failed to update board star.", Severity.Error);
            board.IsStarred = !board.IsStarred;
            ApplyFilterAndSort();
        }
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
        if (e.UserId == _currentUserId)
        {
            allBoards = allBoards.Where(b => b.Id != e.BoardId).ToList();
            rawLastOpenedBoards = rawLastOpenedBoards.Where(b => b.Id != e.BoardId).ToList();
            ApplyFilterAndSort();

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
            rawLastOpenedBoards = tempList.OrderByDescending(x => x.LastModified).Take(4).ToList();
        }
    }

    private void HandleBoardClick(int boardId)
    {
        Nav.NavigateTo($"/board/{boardId}");
    }

    private async void HandleCreateBoard()
    {
        if (IsMaxedOut)
        {
            bool? upgrade = await DialogService.ShowMessageBox(
                "Limit Reached",
                $"You have reached the free tier limit of {_maxFreeBoards} boards. Upgrade to Pro to create unlimited boards!",
                yesText: "Upgrade to Pro",
                cancelText: "Cancel");

            if (upgrade == true)
            {
                await NavigateToUpgrade();
            }
            return;
        }

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

    private async Task UpdateBoardTheme(BoardDto board, BoardThemeColor newColor)
    {
        var oldColor = board.ThemeColor;
        board.ThemeColor = newColor;
        StateHasChanged();

        var result = await BoardsService.UpdateBoardThemeAsync(board.Id, newColor);

        if (!result.IsSuccess)
        {
            board.ThemeColor = oldColor;
            StateHasChanged();
            SnackBar.Add("Failed to update board theme.", Severity.Error);
        }
    }

    public void Dispose()
    {
        InvitationHub.OnInviteResponded -= HandleInviteResponded;
        BoardHub.OnMemberRemoved -= HandleMemberRemoved;
    }
}