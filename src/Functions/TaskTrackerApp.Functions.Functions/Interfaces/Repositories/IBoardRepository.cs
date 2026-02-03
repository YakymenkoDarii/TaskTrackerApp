using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Functions.Functions.Data.Dtos.Board;

namespace TaskTrackerApp.Functions.Functions.Interfaces.Repositories;

public interface IBoardRepository
{
    Task DeleteAsync(int boardId);

    Task<BoardExportDto?> GetFullBoardAsync(int boardId);
}