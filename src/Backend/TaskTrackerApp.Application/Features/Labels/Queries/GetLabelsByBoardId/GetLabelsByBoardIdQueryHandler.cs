using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.LabelMappers;
using TaskTrackerApp.Domain.DTOs.Labels;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Queries.GetLabelsByBoardId;

public class GetLabelsByBoardIdQueryHandler : IRequestHandler<GetLabelsByBoardIdQuery, Result<IEnumerable<LabelDto>>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetLabelsByBoardIdQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<LabelDto>>> Handle(GetLabelsByBoardIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var labels = await uow.LabelsRepository.GetLabelsByBoardIdAsync(request.BoardId);

        var dtos = labels.Select(l => l.ToDto());

        return Result<IEnumerable<LabelDto>>.Success(dtos);
    }
}