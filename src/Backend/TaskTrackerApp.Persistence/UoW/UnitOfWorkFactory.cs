using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.UoW;

public class UnitOfWorkFactory(IDbContextFactory<TaskTrackerDbContext> contextFactory) : IUnitOfWorkFactory
{
    public IUnitOfWork Create()
    {
        var context = contextFactory.CreateDbContext();
        return new UnitOfWork(context);
    }
}