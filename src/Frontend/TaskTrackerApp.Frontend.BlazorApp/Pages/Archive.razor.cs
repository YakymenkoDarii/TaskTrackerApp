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

    private List<ArchivedBoardDto> archivedBoards = new();
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
                archivedBoards = result.Value.OrderByDescending(x => x.Id).ToList();
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

    private async Task HandleRestoreBoard(ArchivedBoardDto board)
    {
        bool? confirmed = await DialogService.ShowMessageBox(
            "Restore Board",
            $"Are you sure you want to restore '{board.Title}'? It will be moved back to your active dashboard. This operation will take some time.",
            yesText: "Restore",
            cancelText: "Cancel");

        if (confirmed == true)
        {
            var result = await BoardsService.UnArchiveBoardAsync(board.Id);

            if (result.IsSuccess)
            {
                Snackbar.Add("Board restored successfully", Severity.Success);

                archivedBoards.Remove(board);
            }
            else
            {
                Snackbar.Add($"Failed to restore board. The error is: {result.Error}", Severity.Error);
            }
        }
    }
}