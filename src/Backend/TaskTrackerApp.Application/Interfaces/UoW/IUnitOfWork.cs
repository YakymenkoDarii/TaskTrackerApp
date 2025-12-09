using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Repositories;

namespace TaskTrackerApp.Application.Interfaces.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        //Add other repositories
        ICardRepository CardRepository { get; }
       // IBoardRepository BoardRepository { get; }
       // IColumnRepository ColumnRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
