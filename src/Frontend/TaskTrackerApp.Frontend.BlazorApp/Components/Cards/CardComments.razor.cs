using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.Domain.DTOs.CardComments;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Components.Cards;

public partial class CardComments
{
    [Inject] private ICardCommentsService CardCommentsService { get; set; }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

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
        await LoadComments();
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
}