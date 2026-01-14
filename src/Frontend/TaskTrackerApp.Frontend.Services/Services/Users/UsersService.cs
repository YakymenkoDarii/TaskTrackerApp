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
}