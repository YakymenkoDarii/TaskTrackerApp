using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskTrackerApp.Application.Interfaces.Common;

namespace TaskTrackerApp.Persistence.Common;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId => int.TryParse(
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
        out var id) ? id : null;
}