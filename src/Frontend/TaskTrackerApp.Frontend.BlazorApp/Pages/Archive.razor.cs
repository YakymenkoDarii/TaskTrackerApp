using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages;

public partial class Archive
{
    [Inject] private IBoardsService BoardsService { get; set; } = default!;

    [Inject] private NavigationManager Nav { get; set; } = default!;

    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    [Inject] private IDialogService DialogService { get; set; } = default!;

    private IEnumerable<BoardDto> archivedBoards = Enumerable.Empty<BoardDto>();
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadArchivedBoardsAsync();
    }

    private async Task LoadArchivedBoardsAsync()
    {
        isLoading = true;
        try
        {
            var result = await BoardsService.GetArchivedAsync();
            if (result.IsSuccess && result.Value is not null)
            {
                archivedBoards = result.Value.OrderByDescending(x => x.LastTimeOpenned);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Snackbar.Add("Failed to load archive", Severity.Error);
        }
        finally
        {
            isLoading = false;
        }
    }

    private void HandleBoardClick(int boardId)
    {
        Nav.NavigateTo($"/board/{boardId}");
    }
}