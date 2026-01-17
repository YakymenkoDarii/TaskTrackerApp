using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.DTOs.Labels;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface ILabelService
{
    Task<Result> AddLabelToCard(int cardId, int id);

    Task<Result> CreateLabel(CreateLabelDto createDto);

    Task<Result<IEnumerable<LabelDto>>> GetLabelsByBoardId(int boardId);

    Task<Result> RemoveLabelFromCard(int cardId, int id);

    Task<Result> UpdateLabel(LabelDto updateDto);
}