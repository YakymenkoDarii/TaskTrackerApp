using System.Net.Http.Headers;
using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.CardComments;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.CardComments;

public class CardCommentsService : ICardCommentsService
{
    private readonly ICardCommentsApi _api;

    public CardCommentsService(ICardCommentsApi api)
    {
        _api = api;
    }

    public async Task<Result> CreateComment(CreateCardCommentDto createDto)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(createDto.CardId.ToString()), nameof(createDto.CardId));
            content.Add(new StringContent(createDto.CreatedById.ToString()), nameof(createDto.CreatedById));
            content.Add(new StringContent(createDto.Text ?? string.Empty), nameof(createDto.Text));

            if (createDto.Files != null)
            {
                foreach (var file in createDto.Files)
                {
                    var memoryStream = new MemoryStream();
                    await file.OpenReadStream(10 * 1024 * 1024).CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var streamContent = new StreamContent(memoryStream);

                    var contentType = string.IsNullOrWhiteSpace(file.ContentType)
                        ? "application/octet-stream"
                        : file.ContentType;

                    try
                    {
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    }
                    catch (FormatException)
                    {
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    }

                    content.Add(streamContent, "Files", file.Name);
                }
            }

            var response = await _api.CreateCommentAsync(content);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> DeleteComment(int id)
    {
        try
        {
            var response = await _api.DeleteCommentAsync(id);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result<IEnumerable<CardCommentDto>>> GetCommentsByCardId(int cardId)
    {
        try
        {
            var response = await _api.GetCommentsByCardIdasync(cardId);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<IEnumerable<CardCommentDto>>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CardCommentDto>>.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> UpdateComment(UpdateCardCommentDto updateDto)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(updateDto.Id.ToString()), nameof(updateDto.Id));
            content.Add(new StringContent(updateDto.Text ?? string.Empty), nameof(updateDto.Text));

            if (updateDto.KeepAttachmentIds != null)
            {
                foreach (var id in updateDto.KeepAttachmentIds)
                {
                    content.Add(new StringContent(id.ToString()), nameof(updateDto.KeepAttachmentIds));
                }
            }

            if (updateDto.NewFiles != null)
            {
                foreach (var file in updateDto.NewFiles)
                {
                    var memoryStream = new MemoryStream();
                    await file.OpenReadStream(10 * 1024 * 1024).CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var streamContent = new StreamContent(memoryStream);

                    var contentType = string.IsNullOrWhiteSpace(file.ContentType)
                        ? "application/octet-stream"
                        : file.ContentType;

                    try
                    {
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    }
                    catch (FormatException)
                    {
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    }

                    content.Add(streamContent, "NewAttachments", file.Name);
                }
            }

            var response = await _api.UpdateCommentAsync(content);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("UnknownError", ex.Message));
        }
    }
}