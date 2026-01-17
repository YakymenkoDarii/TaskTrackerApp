using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.DTOs.Labels;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface ILabelApi
{
    [Post("/api/Labels/create")]
    Task<IApiResponse<Result<LabelDto>>> CreateLabelAsync([Body] CreateLabelDto createDto);

    [Put("/api/Labels/update")]
    Task<IApiResponse<Result<LabelDto>>> UpdateLabelAsync([Body] LabelDto updateDto);

    [Get("/api/Labels/{boardId}")]
    Task<IApiResponse<Result<IEnumerable<LabelDto>>>> GetLabelsByBoardIdAsync(int boardId);

    [Post("/api/Labels/{cardId}/{labelId}")]
    Task<IApiResponse<Result>> AddLabelToCardAsync(int cardId, int labelId);

    [Delete("/api/Labels/{cardId}/{labelId}")]
    Task<IApiResponse<Result>> RemoveLabelFromCardAsync(int cardId, int labelId);
}