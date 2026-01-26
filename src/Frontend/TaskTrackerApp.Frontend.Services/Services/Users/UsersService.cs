using Microsoft.AspNetCore.Components.Forms;
using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Users;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Users;

public class UsersService : IUsersService
{
    private readonly IUsersApi _usersApi;

    public UsersService(IUsersApi usersApi)
    {
        _usersApi = usersApi;
    }

    public async Task<Result<IEnumerable<UserSummaryDto>>> SearchUsersAsync(string searchTerm, CancellationToken token, int? excludeBoardId = null)
    {
        try
        {
            var response = await _usersApi.SearchUsersAsync(searchTerm, excludeBoardId, token);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Result<IEnumerable<UserSummaryDto>>.Success(Enumerable.Empty<UserSummaryDto>());
            }

            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<IEnumerable<UserSummaryDto>>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<UserSummaryDto>>.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> UpdateAsync(UpdateUserDto userDto)
    {
        try
        {
            var response = await _usersApi.UpdateAsync(userDto);
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

    public async Task<Result<Uri>> UpdateAvatarAsync(IBrowserFile file)
    {
        try
        {
            long maxFileSize = 5 * 1024 * 1024;

            using var memoryStream = new MemoryStream();

            using var browserStream = file.OpenReadStream(maxFileSize);

            await browserStream.CopyToAsync(memoryStream);

            memoryStream.Position = 0;

            var streamPart = new StreamPart(memoryStream, file.Name, file.ContentType);

            var response = await _usersApi.UpdateAvatarAsync(streamPart);

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                return Result<Uri>.Success(new Uri(response.Content.Value.AbsoluteUri));
            }

            return Result<Uri>.Failure(new Error("UploadFailed", "Could not upload avatar"));
        }
        catch (ApiException ex)
        {
            return Result<Uri>.Failure(new Error("NetworkError", ex.Message));
        }
        catch (Exception ex)
        {
            return Result<Uri>.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> ChangePassword(ChangePasswordRequest request)
    {
        try
        {
            var response = await _usersApi.ChangePassword(request);
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

    public async Task<Result<MyProfileDto>> GetProfileAsync()
    {
        try
        {
            var response = await _usersApi.GetProfileAsync();
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<MyProfileDto>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<MyProfileDto>.Failure(new Error("UnknownError", ex.Message));
        }
    }
}