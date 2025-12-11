namespace TaskTrackerApp.Application.Interfaces.UoW;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}