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
    Task<Result<LabelDto>> CreateLabelAsync(CreateLabelDto createDto);

    Task<Result<LabelDto>> UpdateLabelAsync(LabelDto updateDto);

    Task<Result<IEnumerable<LabelDto>>> GetLabelsByBoardIdAsync(int boardId);

    Task<Result> AddLabelToCardAsync(int cardId, int labelId);

    Task<Result> RemoveLabelFromCardAsync(int cardId, int labelId);
}