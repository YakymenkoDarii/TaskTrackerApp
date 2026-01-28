using Blazored.TextEditor;
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
using TaskTrackerApp.Frontend.Domain.Models.CommentAttachments;
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
    private List<FilePreviewModel> _selectedFiles = new();

    private DotNetObjectReference<CardComments>? _objRef;

    private int? _editingCommentId = null;
    private BlazoredTextEditor _editQuillHtml;
    private List<int> _keepAttachmentIds = new();
    private List<IBrowserFile> _newEditFiles = new();

    private int _currentUserId;
    private string _currentUserInitials = "?";
    private string? _currentUserAvatarUrl;

    private BlazoredTextEditor _quillHtml;
    private bool _isSaving = false;

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

            var url = $"data:{contentType};base64,{base64Data}";

            if (_editingCommentId != null)
            {
                _newEditFiles.Add(file);
            }
            else
            {
                _selectedFiles.Add(new FilePreviewModel
                {
                    File = file,
                    Url = url,
                    IsImage = true
                });
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

                var base64 = Convert.ToBase64String(buffer);
                var url = $"data:{file.ContentType};base64,{base64}";

                _selectedFiles.Add(new FilePreviewModel
                {
                    File = safeFile,
                    Url = url,
                    IsImage = file.ContentType.StartsWith("image/")
                });
                if (file.ContentType.StartsWith("image/"))
                {
                    await _quillHtml.InsertImage(url);
                }
            }
            else
            {
                Snackbar.Add($"File {file.Name} is too big (Max 10MB)", Severity.Warning);
            }
        }
    }

    private async Task HandlePaste(ClipboardEventArgs e)
    {
        _isFocused = true;
    }

    private void RemoveFile(FilePreviewModel fileModel) => _selectedFiles.Remove(fileModel);

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
        var htmlContent = await _quillHtml.GetHTML();

        if (string.IsNullOrWhiteSpace(htmlContent) && !_selectedFiles.Any()) return;

        _isSaving = true;

        var createDto = new CreateCardCommentDto
        {
            CardId = CardId,
            Text = htmlContent,
            CreatedById = _currentUserId,
            Files = _selectedFiles.Select(f => f.File).ToList()
        };

        var result = await CardCommentsService.CreateComment(createDto);

        if (result.IsSuccess)
        {
            await _quillHtml.LoadHTMLContent("");
            _newCommentText = string.Empty;
            _selectedFiles.Clear();
        }
        else
        {
            Snackbar.Add($"Error: {result.Error.Message}", Severity.Error);
        }

        _isSaving = false;
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

    private async Task StartEdit(CardCommentDto comment)
    {
        _editingCommentId = comment.Id;
        _keepAttachmentIds = comment.Attachments.Select(a => a.Id).ToList();
        _newEditFiles.Clear();

        StateHasChanged();
        await Task.Delay(50);

        if (_editQuillHtml != null)
        {
            await _editQuillHtml.LoadHTMLContent(comment.Text);
        }
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
        try
        {
            var files = e.GetMultipleFiles();
            foreach (var file in files)
            {
                if (file.Size <= 10 * 1024 * 1024)
                {
                    using var stream = file.OpenReadStream(10 * 1024 * 1024);
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    var buffer = ms.ToArray();

                    var safeFile = new CustomBrowserFile(buffer, file.Name, file.ContentType);

                    _newEditFiles.Add(safeFile);

                    if (file.ContentType.StartsWith("image/"))
                    {
                        var base64 = Convert.ToBase64String(buffer);
                        var url = $"data:{file.ContentType};base64,{base64}";
                        if (_editQuillHtml != null)
                        {
                            await _editQuillHtml.InsertImage(url);
                        }
                    }
                }
                else
                {
                    Snackbar.Add($"File {file.Name} is too big (Max 10MB)", Severity.Warning);
                }
            }

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file: {ex.Message}");
            Snackbar.Add("Failed to attach file.", Severity.Error);
        }
    }

    private async Task SaveEdit()
    {
        if (_editingCommentId == null) return;

        var editedHtml = await _editQuillHtml.GetHTML();

        var updateDto = new UpdateCardCommentDto
        {
            Id = _editingCommentId.Value,
            Text = editedHtml,
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
    }

    private string GetFileIcon(string contentType)
    {
        if (string.IsNullOrEmpty(contentType)) return Icons.Material.Filled.InsertDriveFile;
        if (contentType.Contains("image")) return Icons.Material.Filled.Image;
        if (contentType.Contains("pdf")) return Icons.Material.Filled.PictureAsPdf;
        return Icons.Material.Filled.InsertDriveFile;
    }

    private string FormatSize(long bytes)
    {
        var sizeInMb = bytes / 1024f / 1024f;
        if (sizeInMb < 1)
        {
            var sizeInKb = bytes / 1024f;
            return $"{sizeInKb:F0} KB";
        }
        return $"{sizeInMb:F1} MB";
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

    private Color GetFileIconColor(string contentType)
    {
        if (string.IsNullOrEmpty(contentType)) return Color.Default;
        if (contentType.Contains("pdf")) return Color.Error;
        if (contentType.Contains("image")) return Color.Info;
        if (contentType.Contains("word") || contentType.Contains("document")) return Color.Primary; // Blue for Word
        if (contentType.Contains("sheet") || contentType.Contains("excel")) return Color.Success; // Green for Excel
        return Color.Secondary;
    }

    public void Dispose()
    {
        CardHub.OnCommentAdded -= HandleCommentAdded;
        CardHub.OnCommentUpdated -= HandleCommentUpdated;
        CardHub.OnCommentDeleted -= HandleCommentDeleted;
        _objRef?.Dispose();
    }
}