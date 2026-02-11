using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.BlazorApp.Layout;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.CardDialogs;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.ColumnDialogs;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.InvitationDialogs;
using TaskTrackerApp.Frontend.Domain;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.DTOs.Columns;
using TaskTrackerApp.Frontend.Domain.DTOs.Meeting;
using TaskTrackerApp.Frontend.Domain.Enums;
using TaskTrackerApp.Frontend.Domain.Events.BoardMember;
using TaskTrackerApp.Frontend.Domain.Events.Card;
using TaskTrackerApp.Frontend.Domain.Events.Column;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Hubs;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages;

[Authorize]
public partial class Board : IDisposable
{
    [Parameter] public int BoardId { get; set; }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject] private IBoardsService BoardsService { get; set; }

    [Inject] private IColumnsService ColumnsService { get; set; }

    [Inject] private ICardsService CardsService { get; set; }

    [Inject] private IBoardMembersService BoardMembersService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    [Inject] private ILocalStorageService LocalStorage { get; set; }

    [Inject] private NavigationManager Nav { get; set; }

    [Inject] private BoardSignalRService BoardHub { get; set; }

    [Inject] private IJSRuntime JS { get; set; }

    [CascadingParameter] public MainLayout? MainLayout { get; set; }

    [SupplyParameterFromQuery]
    public int? OpenCard { get; set; }

    private BoardDto? board;
    private List<ColumnDto> columns = new();
    private List<CardDto> _allCards = new();
    private bool isLoading = true;
    private bool _isLayoutMode = false;

    private MudDropContainer<ColumnDto> _columnDropContainer;
    private MudDropContainer<CardDto> _cardDropContainer;

    private BoardRole _currentUserRole = BoardRole.Viewer;
    private bool IsAdmin => _currentUserRole == BoardRole.Admin;

    private bool CanEditContent =>
           board != null
           && (_currentUserRole == BoardRole.Admin || _currentUserRole == BoardRole.Member);

    //Meeting fields
    private MeetingDto? CurrentMeeting;

    private bool IsInMeeting = false;
    private string? MyPeerId;
    private List<MeetingParticipant> RemoteParticipants = new();
    private DotNetObjectReference<Board>? _objRef;
    private bool IsMinimized = false;
    private bool IsMuted = true;
    private bool IsVideoOff = true;
    private bool IsScreenSharing = false;

    private List<ScreenShareSession> IncomingScreens = new();

    protected override async Task OnInitializedAsync()
    {
        await AddToRecentBoardsAsync(BoardId);

        await BoardHub.StartConnection();
        RegisterSignalREvents();
        await BoardHub.JoinBoard(BoardId);

        CurrentMeeting = await BoardHub.GetActiveMeetingAsync(BoardId);

        await LoadBoardDataAsync();

        if (OpenCard.HasValue)
        {
            var cardToOpen = _allCards.FirstOrDefault(x => x.Id == OpenCard.Value);
            if (cardToOpen != null) HandleCardClick(cardToOpen);
        }
    }

    private void RegisterSignalREvents()
    {
        BoardHub.OnColumnCreated += async (e) => await InvokeAsync(() => OnColumnCreated(e));
        BoardHub.OnColumnMoved += async (e) => await InvokeAsync(() => OnColumnMoved(e));
        BoardHub.OnColumnDeleted += async (e) => await InvokeAsync(() => OnColumnDeleted(e));
        BoardHub.OnCardCreated += async (e) => await InvokeAsync(() => OnCardCreated(e));
        BoardHub.OnCardMoved += async (e) => await InvokeAsync(() => OnCardMoved(e));
        BoardHub.OnCardUpdated += async (e) => await InvokeAsync(() => OnCardUpdated(e));
        BoardHub.OnCardDeleted += async (e) => await InvokeAsync(() => OnCardDeleted(e));
        BoardHub.OnMemberRoleUpdated += async (e) => await InvokeAsync(() => OnMemberRoleUpdated(e));
        BoardHub.OnMemberRemoved += async (e) => await InvokeAsync(() => OnMemberRemoved(e));

        BoardHub.OnMeetingStateUpdated += async (m) => await InvokeAsync(() =>
        {
            CurrentMeeting = m;
            StateHasChanged();
        });

        BoardHub.OnUserJoinedMeeting += async (participant) => await InvokeAsync(async () =>
        {
            if (IsInMeeting && participant.PeerId != MyPeerId)
            {
                if (!RemoteParticipants.Any(p => p.PeerId == participant.PeerId))
                {
                    RemoteParticipants.Add(participant);
                    StateHasChanged();
                }

                if (IsScreenSharing)
                {
                    Console.WriteLine($"[ScreenShare] New user joined. Sharing screen with {participant.PeerId}...");
                    await JS.InvokeVoidAsync("simpleVideo.shareScreenWithUser", participant.PeerId);
                }
            }
        });

        BoardHub.OnUserLeftMeeting += async (peerId) => await InvokeAsync(() =>
        {
            var participant = RemoteParticipants.FirstOrDefault(p => p.PeerId == peerId);

            if (participant != null)
            {
                RemoteParticipants.Remove(participant);
                StateHasChanged();
            }
        });
        BoardHub.OnJoinMeetingResponse += async (peerIds) => await InvokeAsync(async () =>
        {
            foreach (var peerId in peerIds)
            {
                var fullParticipant = CurrentMeeting?.Participants.FirstOrDefault(p => p.PeerId == peerId);

                if (fullParticipant != null)
                {
                    RemoteParticipants.Add(fullParticipant);
                }
                else
                {
                    RemoteParticipants.Add(new MeetingParticipant
                    {
                        PeerId = peerId,
                        DisplayName = "Connecting...",
                        UserId = 0
                    });
                }
            }

            StateHasChanged();

            await Task.Delay(100);

            await JS.InvokeVoidAsync("simpleVideo.callUsers", peerIds);
        });

        BoardHub.OnParticipantStateUpdated += async (peerId, isMuted, isVideoOff) => await InvokeAsync(async () =>
        {
            var participant = RemoteParticipants.FirstOrDefault(p => p.PeerId == peerId);
            if (participant != null)
            {
                participant.IsMuted = isMuted;
                participant.IsVideoOff = isVideoOff;
                StateHasChanged();

                if (!isVideoOff)
                {
                    await Task.Delay(50);
                    await JS.InvokeVoidAsync("simpleVideo.ensureVideoPlays", peerId);
                }
            }
        });

        BoardHub.OnUserStoppedScreenShare += async (peerId) => await InvokeAsync(() =>
        {
            var screen = IncomingScreens.FirstOrDefault(s => s.PeerId == peerId);
            if (screen != null)
            {
                IncomingScreens.Remove(screen);
                StateHasChanged();
            }
        });
    }

    private async Task OnCardMoved(CardMovedEvent e)
    {
        var card = _allCards.FirstOrDefault(c => c.Id == e.CardId);

        if (card != null && (card.ColumnId != e.NewColumnId || card.Position != e.NewPosition))
        {
            AdjustLocalPositions(e.CardId, e.NewColumnId, e.NewPosition);

            _allCards.Sort((a, b) => a.Position.CompareTo(b.Position));

            _cardDropContainer?.Refresh();
            StateHasChanged();
        }
    }

    private async Task OnCardCreated(CardCreatedEvent e)
    {
        if (_allCards.Any(c => c.Id == e.CardId)) return;

        var newCard = new CardDto
        {
            Id = e.CardId,
            Title = e.Title,
            Description = e.Description,
            ColumnId = e.ColumnId,
            BoardId = e.BoardId,
            AssigneeId = e.AssigneeId,
            DueDate = e.DueDate,
            Position = e.Position,
            Priority = e.Priority
        };

        _allCards.Add(newCard);
        _cardDropContainer?.Refresh();
        StateHasChanged();
    }

    private async Task OnCardUpdated(CardUpdatedEvent e)
    {
        var card = _allCards.FirstOrDefault(c => c.Id == e.CardId);
        if (card != null)
        {
            card.Title = e.Title;
            card.Description = e.Description;
            card.DueDate = e.DueDate;
            card.Priority = e.Priority;
            card.IsCompleted = e.IsCompleted;
            card.AssigneeId = e.AssigneeId;
            StateHasChanged();
            _cardDropContainer.Refresh();
        }
    }

    private async Task OnCardDeleted(CardDeletedEvent e)
    {
        var card = _allCards.FirstOrDefault(c => c.Id == e.Id);
        if (card != null)
        {
            _allCards.Remove(card);
            _cardDropContainer?.Refresh();
            StateHasChanged();
        }
    }

    private async Task OnColumnCreated(ColumnCreatedEvent e)
    {
        Console.WriteLine($"Column title {e.Title}");
        if (columns.Any(c => c.Id == e.Id)) return;

        columns.Add(new ColumnDto
        {
            Id = e.Id,
            Title = e.Title,
            Position = e.Position
        });
        columns = columns.OrderBy(c => c.Position).ToList();
        _columnDropContainer?.Refresh();
        StateHasChanged();
    }

    private async Task OnColumnMoved(ColumnMovedEvent e)
    {
        var col = columns.FirstOrDefault(c => c.Id == e.ColumnId);
        if (col != null)
        {
            AdjustLocalColumnPositions(e.ColumnId, e.NewPosition);
            columns = columns.OrderBy(c => c.Position).ToList();
            _columnDropContainer?.Refresh();
            StateHasChanged();
        }
    }

    private async Task OnColumnDeleted(ColumnDeletedEvent e)
    {
        var col = columns.FirstOrDefault(c => c.Id == e.ColumnId);
        if (col != null)
        {
            columns.Remove(col);
            _allCards.RemoveAll(c => c.ColumnId == e.ColumnId);
            _columnDropContainer?.Refresh();
            StateHasChanged();
        }
    }

    private async Task OnMemberRoleUpdated(BoardMemberRoleUpdatedEvent e)
    {
        var userId = await GetUserId();
        if (e.UserId == userId)
        {
            if (Enum.TryParse<BoardRole>(e.NewRole, true, out var role))
            {
                _currentUserRole = role;
                StateHasChanged();
            }
        }
    }

    private async Task OnMemberRemoved(BoardMemberRemovedEvent e)
    {
        var userId = await GetUserId();
        if (e.UserId == userId)
        {
            Snackbar.Add("You have been removed from this board.", Severity.Warning);
            Nav.NavigateTo("/boards");
        }
    }

    private async Task CardDropped(MudItemDropInfo<CardDto> dropItem)
    {
        Console.WriteLine($"[Drop Start] Item: {dropItem.Item?.Title ?? "null"} | Zone: {dropItem.DropzoneIdentifier} | Index: {dropItem.IndexInZone}");

        if (!CanEditContent) return;

        var cardInList = _allCards.FirstOrDefault(c => c.Id == dropItem.Item.Id);
        if (cardInList == null) return;

        if (!int.TryParse(dropItem.DropzoneIdentifier, out int newColumnId)) return;

        int newPosition = dropItem.IndexInZone;

        AdjustLocalPositions(cardInList.Id, newColumnId, newPosition);

        _allCards.Sort((a, b) => a.Position.CompareTo(b.Position));

        _cardDropContainer?.Refresh();
        StateHasChanged();

        int userId = await GetUserId();
        var updateDto = new UpdateCardDto
        {
            Id = cardInList.Id,
            Title = cardInList.Title,
            Description = cardInList.Description,
            ColumnId = newColumnId,
            DueDate = cardInList.DueDate,
            IsCompleted = cardInList.IsCompleted,
            BoardId = BoardId,
            UpdatedById = userId,
            Position = newPosition,
            Priority = cardInList.Priority
        };

        var result = await CardsService.UpdateAsync(cardInList.Id, updateDto);

        if (!result.IsSuccess)
        {
            Snackbar.Add("Failed to move card", Severity.Error);
            await ReloadCards();
        }
    }

    private async Task ColumnDropped(MudItemDropInfo<ColumnDto> dropItem)
    {
        if (!CanEditContent) return;

        AdjustLocalColumnPositions(dropItem.Item.Id, dropItem.IndexInZone);

        columns = columns.OrderBy(c => c.Position).ToList();
        _columnDropContainer.Refresh();

        int userId = await GetUserId();
        var updateDto = new UpdateColumnDto
        {
            Id = dropItem.Item.Id,
            Title = dropItem.Item.Title,
            Description = dropItem.Item.Description ?? string.Empty,
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

    private void AdjustLocalPositions(int cardId, int newColumnId, int newPosition)
    {
        var card = _allCards.FirstOrDefault(c => c.Id == cardId);
        if (card == null) return;

        var targetColumnCards = _allCards
            .Where(c => c.ColumnId == newColumnId && c.Id != cardId)
            .OrderBy(c => c.Position)
            .ToList();

        if (newPosition >= targetColumnCards.Count)
        {
            targetColumnCards.Add(card);
        }
        else
        {
            targetColumnCards.Insert(newPosition, card);
        }

        card.ColumnId = newColumnId;

        for (int i = 0; i < targetColumnCards.Count; i++)
        {
            targetColumnCards[i].Position = i;
        }
    }

    private void AdjustLocalColumnPositions(int columnId, int newPosition)
    {
        var col = columns.FirstOrDefault(c => c.Id == columnId);
        if (col == null) return;

        int oldPosition = col.Position;

        if (newPosition > oldPosition)
        {
            var colsToShift = columns.Where(c => c.Position > oldPosition && c.Position <= newPosition && c.Id != columnId);
            foreach (var c in colsToShift) c.Position--;
        }
        else if (newPosition < oldPosition)
        {
            var colsToShift = columns.Where(c => c.Position >= newPosition && c.Position < oldPosition && c.Id != columnId);
            foreach (var c in colsToShift) c.Position++;
        }

        col.Position = newPosition;
    }

    private async Task ReloadCards()
    {
        var result = await CardsService.GetCardsByBoardIdAsync(BoardId);
        if (result.IsSuccess)
        {
            _allCards = result.Value.OrderBy(c => c.Position).ToList();
            _cardDropContainer.Refresh();
            StateHasChanged();
        }
    }

    private async Task LoadBoardDataAsync()
    {
        isLoading = true;
        try
        {
            var boardResult = await BoardsService.GetBoardByIdAsync(BoardId);
            if (!boardResult.IsSuccess) return;
            board = boardResult.Value;

            await DetermineUserRole();

            var columnsResult = await ColumnsService.GetByBoardIdAsync(BoardId);
            if (columnsResult.IsSuccess && columnsResult.Value != null)
            {
                columns = columnsResult.Value.OrderBy(c => c.Position).ToList();
                _allCards.Clear();
                var allCardsResult = await CardsService.GetCardsByBoardIdAsync(BoardId);

                if (allCardsResult.IsSuccess)
                {
                    _allCards = allCardsResult.Value.OrderBy(c => c.Position).ToList();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Snackbar.Add("Failed to load board data.", Severity.Error);
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task HandleAddCard(int columnId)
    {
        if (!CanEditContent) return;

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

            var createResult = await CardsService.CreateCardAsync(newCardDto);

            if (createResult.IsSuccess)
            {
                int newId = createResult.Value;

                int newPosition = _allCards.Any(c => c.ColumnId == columnId)
                    ? _allCards.Where(c => c.ColumnId == columnId).Max(c => c.Position) + 1
                    : 0;

                var createdCard = new CardDto
                {
                    Id = newId,
                    Title = newCardInput.Title,
                    Description = newCardInput.Description,
                    ColumnId = columnId,
                    BoardId = BoardId,
                    Position = newPosition,
                    IsCompleted = false,
                    Priority = CardPriority.Low
                };

                _cardDropContainer?.Refresh();
                StateHasChanged();
            }
        }
    }

    private async Task HandleAddColumn()
    {
        if (!CanEditContent) return;

        var dialog = await DialogService.ShowAsync<CreateColumnDialog>("Add List");
        var result = await dialog.Result;

        if (!result.Canceled && result.Data is CreateColumnDto newColumnDto)
        {
            newColumnDto.BoardId = BoardId;

            int nextPosition = columns.Any() ? columns.Max(c => c.Position) + 1 : 0;

            var createResult = await ColumnsService.CreateColumnAsync(newColumnDto);

            if (createResult.IsSuccess)
            {
                int newId = createResult.Value;

                if (!columns.Any(c => c.Id == newId))
                {
                    var createdColumn = new ColumnDto
                    {
                        Id = newId,
                        Title = newColumnDto.Title,
                        Description = newColumnDto.Description,
                        Position = nextPosition
                    };

                    columns.Add(createdColumn);
                    columns = columns.OrderBy(c => c.Position).ToList();

                    _columnDropContainer?.Refresh();
                    StateHasChanged();
                }
            }
        }
    }

    private async Task HandleDeleteColumn(int columnId)
    {
        if (!CanEditContent) return;

        bool? confirm = await DialogService.ShowMessageBox("Delete List", "Are you sure?", yesText: "Delete", cancelText: "Cancel");

        if (confirm == true)
        {
            var result = await ColumnsService.DeleteColumnAsync(columnId);

            if (result.IsSuccess)
            {
                var colToRemove = columns.FirstOrDefault(c => c.Id == columnId);
                if (colToRemove != null)
                {
                    columns.Remove(colToRemove);
                    _allCards.RemoveAll(c => c.ColumnId == columnId);

                    _columnDropContainer?.Refresh();
                    StateHasChanged();
                }
            }
            else
            {
                Snackbar.Add(result.Error.ToString() ?? "Failed to delete column", Severity.Error);
            }
        }
    }

    private async Task HandleToggleComplete(CardDto card)
    {
        if (!CanEditContent) return;

        bool oldState = card.IsCompleted;

        card.IsCompleted = !card.IsCompleted;

        StateHasChanged();
        _cardDropContainer?.Refresh();

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
            Priority = card.Priority
        };

        var result = await CardsService.UpdateAsync(card.Id, updateCard);

        if (!result.IsSuccess)
        {
            card.IsCompleted = oldState;
            StateHasChanged();
            Snackbar.Add("Failed to update status", Severity.Error);
        }
    }

    private async Task HandleCardClick(CardDto card)
    {
        var parameters = new DialogParameters<CardDetailsDialog>
        {
            { x => x.Card, card },
            { x => x.BoardId, BoardId },
            { x => x.IsReadOnly, !CanEditContent }
        };
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };

        var dialog = await DialogService.ShowAsync<CardDetailsDialog>("Card Details", parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            if (result.Data is CardDto updatedCard)
            {
                var index = _allCards.FindIndex(c => c.Id == updatedCard.Id);
                if (index != -1) _allCards[index] = updatedCard;
                _allCards = _allCards.OrderBy(c => c.Position).ToList();
            }
            else if (result.Data is string action && action == "Deleted")
            {
                var itemToRemove = _allCards.FirstOrDefault(c => c.Id == card.Id);
                if (itemToRemove != null) _allCards.Remove(itemToRemove);
            }
        }
        _cardDropContainer?.Refresh();
        StateHasChanged();
    }

    private async Task HandleArchiveBoard()
    {
        if (!IsAdmin) return;

        bool? confirmed = await DialogService.ShowMessageBox(
            "Archive Board",
            "Do you want to archive this board? It will be moved to the archive list and become read-only.",
            yesText: "Archive",
            cancelText: "Cancel",
            options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall });

        if (confirmed == true)
        {
            var result = await BoardsService.ArchiveBoardAsync(BoardId);

            if (result.IsSuccess)
            {
                Snackbar.Add("Board archived successfully", Severity.Success);
                Nav.NavigateTo("/boards");
            }
            else
            {
                Snackbar.Add("Failed to archive board.", Severity.Error);
            }
        }
    }

    private async Task DetermineUserRole()
    {
        var userId = await GetUserId();
        var membersResult = await BoardMembersService.GetMembersAsync(BoardId);
        if (membersResult.IsSuccess)
        {
            var me = membersResult.Value.FirstOrDefault(m => m.UserId == userId);
            _currentUserRole = (me != null && Enum.TryParse<BoardRole>(me.Role, true, out var role)) ? role : BoardRole.Viewer;
        }
    }

    private async Task SaveColumn(ColumnDto column)
    {
        int userId = await GetUserId();
        var updateDto = new UpdateColumnDto { Id = column.Id, Title = column.Title, Description = column.Description, BoardId = BoardId, Position = column.Position, UpdatedById = userId };
        await ColumnsService.UpdateColumnAsync(column.Id, updateDto);
    }

    private async Task UpdateBoardDetails(string val)
    {
        int userId = await GetUserId();
        var updateDto = new UpdateBoardDto { Id = board.Id, Title = board.Title, Description = board.Description, UpdatedById = userId };
        await BoardsService.UpdateAsync(board.Id, updateDto);
    }

    private async Task<int> GetUserId()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        int userId = 0;
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null) int.TryParse(userIdClaim.Value, out userId);
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

    private void OpenShareDialog()
    {
        var parameters = new DialogParameters
        {
            { "BoardId", BoardId },
            { "IsReadOnly", !CanEditContent }
        };
        DialogService.ShowAsync<ShareBoardDialog>("Invite Members", parameters, new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true });
    }

    private bool IsOverdue(CardDto card) => !card.IsCompleted && card.DueDate.HasValue && card.DueDate.Value < DateTime.Now;

    private Color GetPriorityColor(CardPriority priority) => priority switch
    {
        CardPriority.Critical => Color.Error,
        CardPriority.High => Color.Warning,
        CardPriority.Medium => Color.Info,
        _ => Color.Default
    };

    private async Task StartMeeting()
    {
        _objRef = DotNetObjectReference.Create(this);

        var success = await JS.InvokeAsync<bool>("simpleVideo.startLocalStream", IsMuted, IsVideoOff);

        if (!success)
        {
            Snackbar.Add("Could not access camera/mic", Severity.Error);
            return;
        }

        MyPeerId = await JS.InvokeAsync<string>("simpleVideo.init", _objRef);

        IsInMeeting = true;
        IsMinimized = false;
        StateHasChanged();

        await Task.Delay(50);
        await JS.InvokeVoidAsync("simpleVideo.attachLocalVideo");

        await BoardHub.JoinMeetingAsync(BoardId, MyPeerId, IsMuted, IsVideoOff);
    }

    private async Task LeaveMeeting()
    {
        await JS.InvokeVoidAsync("simpleVideo.leave");

        if (MyPeerId != null)
        {
            await BoardHub.LeaveMeetingAsync(BoardId, MyPeerId);
        }

        IsInMeeting = false;
        IsMinimized = false;
        IsMuted = true;
        IsVideoOff = true;
        IsScreenSharing = false;
        RemoteParticipants.Clear();
        MyPeerId = null;

        _objRef?.Dispose();
        _objRef = null;

        StateHasChanged();
    }

    [JSInvokable]
    public async Task OnRemoteStreamAdded(string peerId)
    {
        var participant = RemoteParticipants.FirstOrDefault(p => p.PeerId == peerId);

        if (participant != null)
        {
            participant.IsVideoOff = false;
            StateHasChanged();

            await Task.Delay(50);

            await JS.InvokeVoidAsync("simpleVideo.reloadVideo", peerId);
        }
        else
        {
            RemoteParticipants.Add(new MeetingParticipant
            {
                PeerId = peerId,
                DisplayName = "Connecting...",
                IsVideoOff = false
            });
            StateHasChanged();

            await Task.Delay(50);
            await JS.InvokeVoidAsync("simpleVideo.reloadVideo", peerId);
        }
    }

    private async Task ToggleMute()
    {
        IsMuted = !IsMuted;
        await JS.InvokeVoidAsync("simpleVideo.toggleAudio", !IsMuted);

        if (MyPeerId != null)
            await BoardHub.UpdateMediaStateAsync(BoardId, MyPeerId, IsMuted, IsVideoOff);
    }

    private async Task ToggleCamera()
    {
        IsVideoOff = !IsVideoOff;

        await JS.InvokeVoidAsync("simpleVideo.toggleVideo", !IsVideoOff);

        if (MyPeerId != null)
        {
            await BoardHub.UpdateMediaStateAsync(BoardId, MyPeerId, IsMuted, IsVideoOff);
        }
    }

    private async Task ToggleScreenShare()
    {
        if (IsScreenSharing)
        {
            await JS.InvokeVoidAsync("simpleVideo.stopScreenShare");
            IsScreenSharing = false;

            var myScreen = IncomingScreens.FirstOrDefault(s => s.PeerId == MyPeerId);
            if (myScreen != null) IncomingScreens.Remove(myScreen);

            if (MyPeerId != null)
                await BoardHub.StopScreenShareAsync(BoardId, MyPeerId);

            StateHasChanged();
        }
        else
        {
            var success = await JS.InvokeAsync<bool>("simpleVideo.startScreenShare");
            if (success)
            {
                IsScreenSharing = true;
                var myName = MainLayout?.UserDisplayName ?? "You";
                if (!IncomingScreens.Any(s => s.PeerId == MyPeerId))
                {
                    IncomingScreens.Add(new ScreenShareSession
                    {
                        PeerId = MyPeerId,
                        DisplayName = $"{myName} (You)",
                        StreamId = "local-screen"
                    });
                }

                StateHasChanged();

                await JS.InvokeVoidAsync("simpleVideo.attachLocalScreen", MyPeerId);
            }
        }
    }

    [JSInvokable]
    public async Task OnScreenShareAdded(string peerId, string streamId)
    {
        var user = RemoteParticipants.FirstOrDefault(u => u.PeerId == peerId);
        var name = user?.DisplayName ?? "Unknown User";

        if (!IncomingScreens.Any(s => s.PeerId == peerId))
        {
            IncomingScreens.Add(new ScreenShareSession
            {
                PeerId = peerId,
                StreamId = streamId,
                DisplayName = name
            });
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        BoardHub.OnMeetingStateUpdated -= (m) => InvokeAsync(() => CurrentMeeting = m);

        if (IsInMeeting)
        {
            _ = LeaveMeeting();
        }

        BoardHub.OnColumnCreated -= (e) => InvokeAsync(() => OnColumnCreated(e));
        BoardHub.OnColumnMoved -= (e) => InvokeAsync(() => OnColumnMoved(e));
        BoardHub.OnColumnDeleted -= (e) => InvokeAsync(() => OnColumnDeleted(e));
        BoardHub.OnCardCreated -= (e) => InvokeAsync(() => OnCardCreated(e));
        BoardHub.OnCardMoved -= (e) => InvokeAsync(() => OnCardMoved(e));
        BoardHub.OnCardUpdated -= (e) => InvokeAsync(() => OnCardUpdated(e));
        BoardHub.OnCardDeleted -= (e) => InvokeAsync(() => OnCardDeleted(e));
        BoardHub.OnMemberRoleUpdated -= (e) => InvokeAsync(() => OnMemberRoleUpdated(e));
        BoardHub.OnMemberRemoved -= (e) => InvokeAsync(() => OnMemberRemoved(e));

        _ = BoardHub.LeaveBoard(BoardId);
        _objRef?.Dispose();
    }
}