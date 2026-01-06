using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Boards;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.WebApp.Components.Pages;

public partial class Boards
{
    [Inject]
    public IBoardsService BoardsService { private get; set; } = default!;

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
            "WORK IN PROGRESS",
            Severity.Info);
    }

    private void HandleCreateBoard()
    {
        SnackBar.Add(
            "WORK IN PROGRESS",
            Severity.Info);
    }
}