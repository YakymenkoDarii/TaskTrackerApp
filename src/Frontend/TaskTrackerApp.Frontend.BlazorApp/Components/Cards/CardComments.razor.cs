using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using System.Security.Claims;
using TaskTrackerApp.Frontend.Domain.DTOs.CardComments;
using TaskTrackerApp.Frontend.Domain.DTOs.CommentAttachments;
using TaskTrackerApp.Frontend.Domain.Events.Comment;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Helpers;
using TaskTrackerApp.Frontend.Services.Services.Hubs;

namespace TaskTrackerApp.Frontend.BlazorApp.Components.Cards;

public partial class CardComments : IDisposable
{
    [Inject] private ICardCommentsService CardCommentsService { get; set; }

    [Inject] private IUsersService UsersService { get; set; }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    [Inject] private CardSignalRService CardHub { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Parameter] public int CardId { get; set; }

    [Parameter] public bool IsReadOnly { get; set; }

    private List<CardCommentDto> _comments = new();

    private string _newCommentText = string.Empty;
    private bool _isFocused = false;
    private List<IBrowserFile> _selectedFiles = new();

    private DotNetObjectReference<CardComments>? _objRef;

    private int? _editingCommentId = null;
    private string _editingText = string.Empty;
    private List<int> _keepAttachmentIds = new();
    private List<IBrowserFile> _newEditFiles = new();

    private int _currentUserId;
    private string _currentUserInitials = "?";
    private string? _currentUserAvatarUrl;

    protected override async Task OnInitializedAsync()
    {
        await LoadCurrentUser();

        CardHub.OnCommentAdded += HandleCommentAdded;
        CardHub.OnCommentUpdated += HandleCommentUpdated;
        CardHub.OnCommentDeleted += HandleCommentDeleted;

        await LoadComments();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _objRef = DotNetObjectReference.Create(this);
            try
            {
                await JSRuntime.InvokeVoidAsync("clipboardHelper.initialize", _objRef);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JS Init failed (ignore if prerendering): {ex.Message}");
            }
        }
    }

    [JSInvokable]
    public void OnImagePasted(string base64Data, string name, string contentType)
    {
        try
        {
            var bytes = Convert.FromBase64String(base64Data);
            var file = new CustomBrowserFile(bytes, name, contentType);

            if (_editingCommentId != null)
            {
                _newEditFiles.Add(file);
            }
            else
            {
                _selectedFiles.Add(file);
                _isFocused = true;
            }

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error pasting image: {ex.Message}");
        }
    }

    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        var files = e.GetMultipleFiles();
        foreach (var file in files)
        {
            if (file.Size <= 10 * 1024 * 1024)
            {
                var buffer = new byte[file.Size];
                await file.OpenReadStream(10 * 1024 * 1024).ReadAsync(buffer);

                var safeFile = new CustomBrowserFile(buffer, file.Name, file.ContentType);

                _selectedFiles.Add(safeFile);
            }
            else
            {
                Snackbar.Add($"File {file.Name} is too big (Max 10MB)", Severity.Warning);
            }
        }
        _isFocused = true;
    }

    private async Task HandlePaste(ClipboardEventArgs e)
    {
        _isFocused = true;
    }

    private void RemoveFile(IBrowserFile file) => _selectedFiles.Remove(file);

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
                    AuthorAvatarUrl = e.AvatarUrl,
                    Attachments = e.Attachments ?? new()
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
            if (e.CardId == CardId)
            {
                var comment = _comments.FirstOrDefault(c => c.Id == e.Id);
                if (comment != null)
                {
                    comment.Text = e.Text;
                    comment.IsEdited = true;
                    comment.UpdatedAt = e.UpdatedAt;
                    comment.Attachments = e.Attachments ?? new();
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

            var result = await UsersService.GetProfileAsync();
            if (result.IsSuccess)
            {
                _currentUserAvatarUrl = result.Value.AvatarUrl;

                if (!string.IsNullOrEmpty(result.Value.DisplayName))
                {
                    _currentUserInitials = GetInitials(result.Value.DisplayName);
                }
            }
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
        if (string.IsNullOrWhiteSpace(_newCommentText) && !_selectedFiles.Any()) return;

        var createDto = new CreateCardCommentDto
        {
            CardId = CardId,
            Text = _newCommentText,
            CreatedById = _currentUserId,
            Files = _selectedFiles.ToList()
        };

        var result = await CardCommentsService.CreateComment(createDto);

        if (result.IsSuccess)
        {
            _newCommentText = string.Empty;
            _selectedFiles.Clear();
            _isFocused = false;
        }
        else
        {
            Snackbar.Add($"Error: {result.Error.Message}", Severity.Error);
            Console.WriteLine($"Create Failed: {result.Error.Code} - {result.Error.Message}");
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

    private void StartEdit(CardCommentDto comment)
    {
        _editingCommentId = comment.Id;
        _editingText = comment.Text;
        _keepAttachmentIds = comment.Attachments.Select(a => a.Id).ToList();
        _newEditFiles.Clear();
    }

    private void ToggleKeepFile(int id)
    {
        if (_keepAttachmentIds.Contains(id))
            _keepAttachmentIds.Remove(id);
        else
            _keepAttachmentIds.Add(id);
    }

    private async Task HandleEditFilesSelected(InputFileChangeEventArgs e)
    {
        var files = e.GetMultipleFiles();
        foreach (var file in files)
        {
            if (file.Size <= 10 * 1024 * 1024)
            {
                var buffer = new byte[file.Size];
                await file.OpenReadStream(10 * 1024 * 1024).ReadAsync(buffer);

                var safeFile = new CustomBrowserFile(buffer, file.Name, file.ContentType);

                _newEditFiles.Add(safeFile);
            }
            else
            {
                Snackbar.Add($"File {file.Name} is too big (Max 10MB)", Severity.Warning);
            }
        }
    }

    private async Task SaveEdit()
    {
        if (_editingCommentId == null) return;

        var updateDto = new UpdateCardCommentDto
        {
            Id = _editingCommentId.Value,
            Text = _editingText,
            KeepAttachmentIds = _keepAttachmentIds,
            NewFiles = _newEditFiles
        };

        var result = await CardCommentsService.UpdateComment(updateDto);

        if (result.IsSuccess)
        {
            Snackbar.Add("Comment updated", Severity.Success);
            CancelEdit();
        }
        else
        {
            Snackbar.Add($"Error: {result.Error.Message}", Severity.Error);
            Console.WriteLine($"Update Failed: {result.Error.Code} - {result.Error.Message}");
        }
    }

    private void CancelEdit()
    {
        _editingCommentId = null;
        _editingText = string.Empty;
    }

    private string GetFileIcon(string? contentType)
    {
        if (string.IsNullOrEmpty(contentType)) return Icons.Material.Filled.InsertDriveFile;
        if (contentType.Contains("pdf")) return Icons.Material.Filled.PictureAsPdf;
        if (contentType.Contains("image")) return Icons.Material.Filled.Image;
        if (contentType.Contains("zip") || contentType.Contains("rar")) return Icons.Material.Filled.FolderZip;
        return Icons.Material.Filled.InsertDriveFile;
    }

    private string FormatSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{Math.Round(bytes / 1024.0, 1)} KB";
        return $"{Math.Round(bytes / 1024.0 / 1024.0, 1)} MB";
    }

    private async Task OpenAttachment(string url)
    {
        await JSRuntime.InvokeVoidAsync("open", url, "_blank");
    }

    private void OnInputBlur()
    {
        Task.Delay(200).ContinueWith(_ =>
        {
            if (string.IsNullOrWhiteSpace(_newCommentText) && !_selectedFiles.Any())
            {
                _isFocused = false;
                InvokeAsync(StateHasChanged);
            }
        });
    }

    private bool IsImage(CommentAttachmentDto att)
    {
        if (!string.IsNullOrEmpty(att.ContentType) && att.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return true;

        var ext = Path.GetExtension(att.FileName)?.ToLowerInvariant();
        return ext is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".bmp";
    }

    public void Dispose()
    {
        CardHub.OnCommentAdded -= HandleCommentAdded;
        CardHub.OnCommentUpdated -= HandleCommentUpdated;
        CardHub.OnCommentDeleted -= HandleCommentDeleted;
        _objRef?.Dispose();
    }
}