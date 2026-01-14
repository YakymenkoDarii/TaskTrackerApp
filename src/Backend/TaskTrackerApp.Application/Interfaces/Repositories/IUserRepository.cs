using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User, int>
{
    Task<User> GetByEmailAsync(string email);

    Task<User> GetByTagAsync(string tag);

    Task<User?> GetByRefreshTokenAsync(string refreshToken);

    Task<IEnumerable<User>> SearchAsync(string term, int? excludeBoardId = null);
}