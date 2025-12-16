using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User, int>
{
    Task<User> GetByEmailAsync(string email);

    Task<User> GetByTagAsync(string tag);
}