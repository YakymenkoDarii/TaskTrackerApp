using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken, int>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
}