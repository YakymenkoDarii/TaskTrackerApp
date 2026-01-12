using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.InvitationDialogs;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages;

public partial class Home
{
    [Inject] private NavigationManager Nav { get; set; } = default!;

    [Inject] private ICardsService CardService { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    [Inject] private IBoardInvitationsService InvitationService { get; set; }

    private DateTime _anchorDate = DateTime.Today;
    private DateTime _weekStart;
    private DateTime _weekEnd;
    private bool _isLoading = false;

    private List<UpcomingCardDto> _tasks = new();

    private List<UpcomingCardDto> _overdueTasks = new();

    private int _pendingInvitesCount = 0;

    protected override async Task OnInitializedAsync()
    {
        CalculateWeekRange();
        await Task.WhenAll(LoadDataAsync(), UpdateInvitationCount());
    }

    private async Task UpdateInvitationCount()
    {
        var result = await InvitationService.GetMyPendingInvitations();
        if (result.IsSuccess)
        {
            _pendingInvitesCount = result.Value.Count();
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
        }
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
}