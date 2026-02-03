using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Functions.Functions.Interfaces.Services;

public interface IBoardArchivalService
{
    Task ArchiveBoard(int boardId);
}