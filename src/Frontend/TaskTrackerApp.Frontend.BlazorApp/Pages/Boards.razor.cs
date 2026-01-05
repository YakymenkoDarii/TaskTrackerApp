using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages;

public partial class Boards
{
    [Inject]
    public IBoardsService BoardsService { private get; set; } = default!;

    [Inject]
    public IDialogService DialogService { private get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { private get; set; } = default!;

    private IEnumerable<BoardDto> lastOpenedBoards = Enumerable.Empty<BoardDto>();
    private IEnumerable<BoardDto> allBoards = Enumerable.Empty<BoardDto>();
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
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

                lastOpenedBoards = data
                    .OrderByDescending(x => x.LastTimeOpenned)
                    .Take(4);

                allBoards = data
                    .OrderBy(x => x.Title);
            }
            else
            {
                SnackBar.Add(result.Error.Message, Severity.Error);
            }
        }
        finally
        {
            isLoading = false;
        }
    }

    private void HandleBoardClick(int boardId)
    {
        SnackBar.Add(
            "BOARD CLICKED",
            Severity.Info);
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

    private void ArchiveBoard(int boardId)
    {
        SnackBar.Add("Archived (Not implemented yet)", Severity.Info);
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
                await LoadBoardsAsync();
                StateHasChanged();
            }
            else
            {
                SnackBar.Add("Failed to delete board", Severity.Error);
            }
        }
    }
}