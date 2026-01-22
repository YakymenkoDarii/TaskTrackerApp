using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.InvitationDialogs;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.Events.Card;
using TaskTrackerApp.Frontend.Domain.Events.Invitations;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Hubs;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages;

public partial class Home : IDisposable
{
    [Inject] private NavigationManager Nav { get; set; } = default!;

    [Inject] private ICardsService CardService { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    [Inject] private IBoardInvitationsService InvitationService { get; set; }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject] private InvitationSignalRService SignalRService { get; set; }

    [Inject] private BoardSignalRService BoardHub { get; set; }

    private DateTime _anchorDate = DateTime.Today;
    private DateTime _weekStart;
    private DateTime _weekEnd;
    private bool _isLoading = false;

    private List<UpcomingCardDto> _tasks = new();
    private List<UpcomingCardDto> _overdueTasks = new();
    private HashSet<int> _activeBoardIds = new();

    private int _pendingInvitesCount = 0;
    private int _currentUserId;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity.IsAuthenticated)
        {
            if (int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out _currentUserId))
            {
                SignalRService.OnInviteReceived += HandleInviteReceived;
                SignalRService.OnInviteRevoked += HandleInviteRevoked;

                BoardHub.OnCardUpdated += HandleCardUpdated;
                BoardHub.OnCardDeleted += HandleCardDeleted;
            }
        }

        CalculateWeekRange();
        await Task.WhenAll(LoadDataAsync(), UpdateInvitationCount());
    }

    private async Task LoadDataAsync()
    {
        _isLoading = true;
        try
        {
            var result = await CardService.GetUpcoming(_weekStart, _weekEnd, includeOverdue: true);

            if (result.IsSuccess && result.Value != null)
            {
                var allData = result.Value;

                _overdueTasks = allData
                    .Where(t => t.DueDate.HasValue
                                && t.DueDate.Value.Date < DateTime.Today
                                && !t.IsCompleted)
                    .OrderBy(t => t.DueDate)
                    .ToList();

                _tasks = allData
                    .Where(t => t.DueDate.HasValue
                                && t.DueDate.Value.Date >= _weekStart
                                && t.DueDate.Value.Date <= _weekEnd)
                    .ToList();

                var visibleBoardIds = allData.Select(t => t.BoardId).Distinct();

                foreach (var boardId in visibleBoardIds)
                {
                    if (!_activeBoardIds.Contains(boardId))
                    {
                        await BoardHub.JoinBoard(boardId);
                        _activeBoardIds.Add(boardId);
                    }
                }
            }
            else
            {
                _tasks = new();
                _overdueTasks = new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private void HandleCardUpdated(CardUpdatedEvent e)
    {
        var task = _tasks.Concat(_overdueTasks).FirstOrDefault(t => t.Id == e.CardId);

        if (task != null)
        {
            task.Title = e.Title;
            task.IsCompleted = e.IsCompleted;
            StateHasChanged();
        }
    }

    private void HandleCardDeleted(CardDeletedEvent e)
    {
        var inTasks = _tasks.FirstOrDefault(t => t.Id == e.Id);
        if (inTasks != null) _tasks.Remove(inTasks);

        var inOverdue = _overdueTasks.FirstOrDefault(t => t.Id == e.Id);
        if (inOverdue != null) _overdueTasks.Remove(inOverdue);

        StateHasChanged();
    }

    private async void HandleInviteReceived(InvitationReceivedEvent _)
    {
        await UpdateInvitationCount();
        StateHasChanged();
    }

    private async void HandleInviteRevoked(int _)
    {
        await UpdateInvitationCount();
        StateHasChanged();
    }

    private async Task UpdateInvitationCount()
    {
        var result = await InvitationService.GetMyPendingInvitations();
        if (result.IsSuccess)
        {
            _pendingInvitesCount = result.Value.Count();
            StateHasChanged();
        }
    }

    private async Task OpenInvitationsDialog()
    {
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            CloseButton = true
        };

        var dialog = DialogService.Show<InvitationsDialog>("My Invitations", options);
        var result = await dialog.Result;

        await UpdateInvitationCount();
    }

    private void CalculateWeekRange()
    {
        int diff = (7 + (_anchorDate.DayOfWeek - DayOfWeek.Monday)) % 7;
        _weekStart = _anchorDate.AddDays(-1 * diff).Date;
        _weekEnd = _weekStart.AddDays(6).Date;
    }

    private async Task PreviousWeek()
    {
        _anchorDate = _anchorDate.AddDays(-7);
        CalculateWeekRange();
        await LoadDataAsync();
    }

    private async Task NextWeek()
    {
        _anchorDate = _anchorDate.AddDays(7);
        CalculateWeekRange();
        await LoadDataAsync();
    }

    private async Task GoToToday()
    {
        _anchorDate = DateTime.Today;
        CalculateWeekRange();
        await LoadDataAsync();
    }

    private bool IsCurrentWeek()
    {
        var today = DateTime.Today;
        return today >= _weekStart && today <= _weekEnd;
    }

    private void NavigateToTask(UpcomingCardDto task)
    {
        Nav.NavigateTo($"/board/{task.BoardId}?openCard={task.Id}");
    }

    private async Task ToggleComplete(UpcomingCardDto task)
    {
        task.IsCompleted = !task.IsCompleted;

        if (task.IsCompleted && _overdueTasks.Contains(task))
        {
            _overdueTasks.Remove(task);
        }

        await CardService.UpdateStatus(task.Id, task.IsCompleted);
        StateHasChanged();
    }

    public void Dispose()
    {
        SignalRService.OnInviteReceived -= HandleInviteReceived;
        SignalRService.OnInviteRevoked -= HandleInviteRevoked;

        BoardHub.OnCardUpdated -= HandleCardUpdated;
        BoardHub.OnCardDeleted -= HandleCardDeleted;

        foreach (var boardId in _activeBoardIds)
        {
            _ = BoardHub.LeaveBoard(boardId);
        }
    }
}