using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.Domain.DTOs.CardComments;
using TaskTrackerApp.Frontend.Domain.Events.Comment;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Hubs;

namespace TaskTrackerApp.Frontend.BlazorApp.Components.Cards;

public partial class CardComments : IDisposable
{
    [Inject] private ICardCommentsService CardCommentsService { get; set; }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    [Inject] private CardSignalRService CardHub { get; set; }

    [Parameter] public int CardId { get; set; }

    [Parameter] public bool IsReadOnly { get; set; }

    private List<CardCommentDto> _comments = new();

    private string _newCommentText = string.Empty;
    private bool _isFocused = false;

    private int? _editingCommentId = null;
    private string _editingText = string.Empty;

    private int _currentUserId;
    private string _currentUserInitials = "?";

    protected override async Task OnInitializedAsync()
    {
        await LoadCurrentUser();

        CardHub.OnCommentAdded += HandleCommentAdded;
        CardHub.OnCommentUpdated += HandleCommentUpdated;
        CardHub.OnCommentDeleted += HandleCommentDeleted;

        await LoadComments();
    }

    private void HandleCommentAdded(CommentAddedEvent e)
    {
        InvokeAsync(() =>
        {
            if (e.CardId == CardId && !_comments.Any(c => c.Id == e.Id))
            {
                var newComment = new CardCommentDto
                {
                    Id = e.Id,
                    Text = e.Text,
                    CreatedAt = e.CreatedAt,
                    CreatedById = e.CreatedById,
                    AuthorName = e.CreatedByName,
                    AuthorAvatarUrl = e.AvatarUrl
                };

                _comments.Insert(0, newComment);
                _comments = _comments.OrderByDescending(c => c.CreatedAt).ToList();
                StateHasChanged();
            }
        });
    }

    private void HandleCommentUpdated(CommentUpdatedEvent e)
    {
        InvokeAsync(() =>
        {
            if (e.CardId == CardId)
            {
                var comment = _comments.FirstOrDefault(c => c.Id == e.Id);

                if (comment != null)
                {
                    comment.Text = e.Text;
                    comment.IsEdited = true;
                    comment.UpdatedAt = e.UpdatedAt;
                    StateHasChanged();
                }
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

    private async Task LoadCurrentUser()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity.IsAuthenticated)
        {
            int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out _currentUserId);
            var name = user.FindFirst(ClaimTypes.Name)?.Value ?? "?";
            _currentUserInitials = GetInitials(name);
        }
    }

    private string GetInitials(string name) =>
        string.IsNullOrEmpty(name) ? "?" : name.Substring(0, 1).ToUpper();

    private async Task LoadComments()
    {
        var result = await CardCommentsService.GetCommentsByCardId(CardId);
        if (result.IsSuccess)
        {
            _comments = result.Value.OrderByDescending(c => c.CreatedAt).ToList();
        }
    }

    private async Task AddComment()
    {
        if (string.IsNullOrWhiteSpace(_newCommentText)) return;

        var createDto = new CreateCardCommentDto
        {
            CardId = CardId,
            Text = _newCommentText,
            CreatedById = _currentUserId
        };

        var result = await CardCommentsService.CreateComment(createDto);

        if (result.IsSuccess)
        {
            _newCommentText = string.Empty;
            _isFocused = false;
            await LoadComments();
        }
        else
        {
            Snackbar.Add("Failed to post comment", Severity.Error);
        }
    }

    private async Task DeleteComment(CardCommentDto comment)
    {
        var confirm = await DialogService.ShowMessageBox("Delete Comment", "Are you sure?", yesText: "Delete", cancelText: "Cancel");
        if (confirm == true)
        {
            var result = await CardCommentsService.DeleteComment(comment.Id);
            if (result.IsSuccess)
            {
                _comments.Remove(comment);
                StateHasChanged();
            }
        }
    }

    private void OnInputBlur()
    {
        Task.Delay(200).ContinueWith(_ =>
        {
            if (string.IsNullOrWhiteSpace(_newCommentText))
            {
                _isFocused = false;
                InvokeAsync(StateHasChanged);
            }
        });
    }

    private void StartEdit(CardCommentDto comment)
    {
        _editingCommentId = comment.Id;
        _editingText = comment.Text;
    }

    private void CancelEdit()
    {
        _editingCommentId = null;
        _editingText = string.Empty;
    }

    private async Task SaveEdit()
    {
        if (_editingCommentId == null || string.IsNullOrWhiteSpace(_editingText)) return;

        var updateDto = new UpdateCardCommentDto
        {
            Id = _editingCommentId.Value,
            Text = _editingText
        };

        var result = await CardCommentsService.UpdateComment(updateDto);

        if (result.IsSuccess)
        {
            var comment = _comments.FirstOrDefault(c => c.Id == _editingCommentId);
            if (comment != null)
            {
                comment.Text = _editingText;
                comment.IsEdited = true;
                comment.UpdatedAt = DateTime.UtcNow;
            }

            Snackbar.Add("Comment updated", Severity.Success);
            CancelEdit();
        }
        else
        {
            Snackbar.Add("Failed to update comment", Severity.Error);
        }
    }

    public void Dispose()
    {
        CardHub.OnCommentAdded -= HandleCommentAdded;
        CardHub.OnCommentUpdated -= HandleCommentUpdated;
        CardHub.OnCommentDeleted -= HandleCommentDeleted;
    }
}