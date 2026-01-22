using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Frontend.Domain.DTOs.CardComments;
using TaskTrackerApp.Frontend.Domain.DTOs.Cards;
using TaskTrackerApp.Frontend.Domain.DTOs.Labels;
using TaskTrackerApp.Frontend.Domain.Enums;
using TaskTrackerApp.Frontend.Domain.Events.BoardMember;
using TaskTrackerApp.Frontend.Domain.Events.Card;
using TaskTrackerApp.Frontend.Domain.Events.Comment;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Hubs;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages.Dialogs.CardDialogs;

public partial class CardDetailsDialog : IAsyncDisposable
{
    [Inject] private ICardsService CardsService { get; set; }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    [Inject] private IBoardMembersService BoardMembersService { get; set; }

    [Inject] private ILabelService LabelService { get; set; }

    [Inject] private BoardSignalRService BoardHub { get; set; }

    [Inject] private CardSignalRService CardHub { get; set; }

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] public CardDto Card { get; set; } = default!;

    [Parameter] public int BoardId { get; set; }

    [Parameter] public bool IsReadOnly { get; set; }

    private string title = string.Empty;
    private string description = string.Empty;
    private DateTime? dueDate;
    private bool isCompleted;
    private CardPriority _priority = CardPriority.Low;
    private int? _assigneeId;
    private List<BoardMemberDto> _boardMembers = new();

    private List<CardCommentDto> _comments = new();

    private List<LabelDto> _allBoardLabels = new();

    protected override async Task OnInitializedAsync()
    {
        if (Card != null)
        {
            UpdateLocalState(Card);
        }

        BoardHub.OnCardUpdated += HandleCardUpdated;
        BoardHub.OnMemberAdded += HandleMemberAdded;

        await CardHub.StartConnection();
        await CardHub.JoinCard(Card.Id);

        CardHub.OnCommentAdded += HandleCommentAdded;
        CardHub.OnCommentUpdated += HandleCommentUpdated;
        CardHub.OnCommentDeleted += HandleCommentDeleted;
        CardHub.OnLabelAdded += HandleLabelAdded;
        CardHub.OnLabelRemoved += HandleLabelRemoved;

        await LoadBoardMembers();
        await LoadBoardLabels();
    }

    private async Task LoadBoardLabels()
    {
        var result = await LabelService.GetLabelsByBoardIdAsync(BoardId);
        if (result.IsSuccess)
        {
            _allBoardLabels = result.Value.ToList();
        }
    }

    private void HandleCommentAdded(CommentAddedEvent e)
    {
        InvokeAsync(() =>
        {
            if (e.CardId == Card.Id)
            {
                if (_comments.Any(c => c.Id == e.Id)) return;

                var newComment = new CardCommentDto
                {
                    Id = e.Id,
                    IsEdited = false,
                    Text = e.Text,
                    CreatedById = e.CreatedById,
                    AuthorName = e.CreatedByName,
                    AuthorAvatarUrl = e.AvatarUrl,
                    CreatedAt = e.CreatedAt
                };

                _comments.Insert(0, newComment);
                StateHasChanged();
            }
        });
    }

    private void HandleCommentUpdated(CommentUpdatedEvent e)
    {
        InvokeAsync(() =>
        {
            var comment = _comments.FirstOrDefault(c => c.Id == e.Id);
            if (comment != null)
            {
                comment.Text = e.Text;
                StateHasChanged();
            }
        });
    }

    private void HandleCommentDeleted(int commentId)
    {
        InvokeAsync(() =>
        {
            var comment = _comments.FirstOrDefault(c => c.Id == commentId);
            if (comment != null)
            {
                _comments.Remove(comment);
                StateHasChanged();
            }
        });
    }

    private void HandleLabelAdded(int cardId, int labelId)
    {
        InvokeAsync(() =>
        {
            if (cardId == Card.Id && !Card.Labels.Any(l => l.Id == labelId))
            {
                var labelDef = _allBoardLabels.FirstOrDefault(l => l.Id == labelId);
                if (labelDef != null)
                {
                    Card.Labels.Add(labelDef);
                    StateHasChanged();
                }
            }
        });
    }

    private void HandleLabelRemoved(int cardId, int labelId)
    {
        InvokeAsync(() =>
        {
            if (cardId == Card.Id)
            {
                var label = Card.Labels.FirstOrDefault(l => l.Id == labelId);
                if (label != null)
                {
                    Card.Labels.Remove(label);
                    StateHasChanged();
                }
            }
        });
    }

    private void UpdateLocalState(CardDto dto)
    {
        title = dto.Title;
        description = dto.Description;
        dueDate = dto.DueDate;
        isCompleted = dto.IsCompleted;
        _assigneeId = dto.AssigneeId;
        _priority = dto.Priority;
    }

    private void HandleCardUpdated(CardUpdatedEvent e)
    {
        InvokeAsync(() =>
        {
            if (e.CardId == Card.Id)
            {
                title = e.Title;
                description = e.Description;
                isCompleted = e.IsCompleted;
                dueDate = e.DueDate;
                _priority = e.Priority;
                _assigneeId = e.AssigneeId;

                Card.Title = e.Title;
                Card.Description = e.Description;
                Card.IsCompleted = e.IsCompleted;
                Card.DueDate = e.DueDate;
                Card.Priority = e.Priority;
                Card.AssigneeId = e.AssigneeId;

                StateHasChanged();
            }
        });
    }

    private async void HandleMemberAdded(BoardMemberAddedEvent e)
    {
        await InvokeAsync(async () =>
        {
            if (e.BoardId == BoardId)
            {
                await LoadBoardMembers();
            }
        });
    }

    private async Task LoadBoardMembers()
    {
        var result = await BoardMembersService.GetMembersAsync(BoardId);
        if (result.IsSuccess)
        {
            _boardMembers = result.Value.ToList();
            StateHasChanged();
        }
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task SaveChanges()
    {
        if (IsReadOnly) return;

        if (string.IsNullOrWhiteSpace(title))
        {
            Snackbar.Add("Title is required", Severity.Warning);
            return;
        }

        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userIdStr = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdStr, out int userId))
        {
            Snackbar.Add("User not authenticated", Severity.Error);
            return;
        }

        var updateDto = new UpdateCardDto
        {
            Id = Card.Id,
            Title = title,
            Description = description,
            DueDate = dueDate,
            ColumnId = Card.ColumnId,
            BoardId = BoardId,
            IsCompleted = isCompleted,
            UpdatedById = userId,
            AssigneeId = _assigneeId,
            Position = Card.Position,
            Priority = _priority,
        };

        var result = await CardsService.UpdateAsync(Card.Id, updateDto);

        if (result.IsSuccess)
        {
            Snackbar.Add("Card updated", Severity.Success);

            Card.Title = title;
            Card.Description = description;
            Card.DueDate = dueDate;
            Card.IsCompleted = isCompleted;
            Card.AssigneeId = _assigneeId;
            Card.Priority = _priority;

            MudDialog.Close(DialogResult.Ok(Card));
        }
        else
        {
            Snackbar.Add(result.Error.Code, Severity.Error);
        }
    }

    private async Task DeleteCard()
    {
        if (IsReadOnly) return;

        bool? result = await DialogService.ShowMessageBox(
            "Delete Card",
            "Are you sure you want to delete this card? This cannot be undone.",
            yesText: "Delete", cancelText: "Cancel");

        if (result != true)
            return;

        var deleteResult = await CardsService.DeleteCardAsync(Card.Id);

        if (deleteResult.IsSuccess)
        {
            Snackbar.Add("Card deleted", Severity.Success);
            MudDialog.Close(DialogResult.Ok("Deleted"));
        }
        else
        {
            Snackbar.Add(deleteResult.Error.Code, Severity.Error);
        }
    }

    private async Task OnCompletionToggled(bool newValue)
    {
        bool oldState = isCompleted;
        isCompleted = newValue;

        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userIdStr = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(userIdStr, out int userId);

        var updateDto = new UpdateCardDto
        {
            Id = Card.Id,
            Title = title,
            Description = description,
            DueDate = dueDate,
            ColumnId = Card.ColumnId,
            BoardId = BoardId,
            IsCompleted = isCompleted,
            UpdatedById = userId,
            AssigneeId = _assigneeId,
            Position = Card.Position,
            Priority = _priority,
        };

        var result = await CardsService.UpdateAsync(Card.Id, updateDto);

        if (result.IsSuccess)
        {
            Snackbar.Add(isCompleted ? "Marked as complete" : "Marked as incomplete", Severity.Success);
            Card.IsCompleted = isCompleted;
        }
        else
        {
            isCompleted = oldState;
            Snackbar.Add("Failed to update status", Severity.Error);
        }
    }

    private Color GetPriorityColor(CardPriority p) => p switch
    {
        CardPriority.Critical => Color.Error,
        CardPriority.High => Color.Warning,
        CardPriority.Medium => Color.Info,
        _ => Color.Default
    };

    public async ValueTask DisposeAsync()
    {
        BoardHub.OnCardUpdated -= HandleCardUpdated;
        BoardHub.OnMemberAdded -= HandleMemberAdded;

        CardHub.OnCommentAdded -= HandleCommentAdded;
        CardHub.OnCommentUpdated -= HandleCommentUpdated;
        CardHub.OnCommentDeleted -= HandleCommentDeleted;

        CardHub.OnLabelAdded -= HandleLabelAdded;
        CardHub.OnLabelRemoved -= HandleLabelRemoved;

        if (Card != null)
        {
            await CardHub.LeaveCard(Card.Id);
        }
    }
}