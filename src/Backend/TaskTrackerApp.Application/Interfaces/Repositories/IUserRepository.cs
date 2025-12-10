using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;
public interface IUserRepository : IRepository<User, int>
{
}
