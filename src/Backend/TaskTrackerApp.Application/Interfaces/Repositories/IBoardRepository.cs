using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories
{
    public interface IBoardRepository : IRepository<Board, int>
    {
    }
}
