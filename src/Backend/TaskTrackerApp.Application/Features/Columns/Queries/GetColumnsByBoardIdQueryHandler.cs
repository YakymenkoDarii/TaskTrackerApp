using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Column;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Columns.Queries;

public class GetColumnsByBoardIdQueryHandler : IRequestHandler<GetColumnsByBoardIdQuery, Result<IEnumerable<ColumnDto>>>
{
    private IUnitOfWorkFactory _uowFactory;

    public GetColumnsByBoardIdQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<ColumnDto>>> Handle(GetColumnsByBoardIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var columns = await uow.ColumnRepository.GetColumnsByBoardIdAsync(request.Id);

        if (columns == null)
        {
            return null;
        }
        var columnDtos = columns.Select(c => new ColumnDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Position = c.Position,
        })
        .OrderBy(p => p.Position)
        .ToList();
        return columnDtos;
    }
}