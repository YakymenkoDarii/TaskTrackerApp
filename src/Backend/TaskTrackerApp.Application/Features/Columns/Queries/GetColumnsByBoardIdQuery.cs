using MediatR;
using TaskTrackerApp.Domain.DTOs.Column;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Columns.Queries;

public class GetColumnsByBoardIdQuery : IRequest<Result<IEnumerable<ColumnDto>>>
{
    public int Id { get; set; }

    public GetColumnsByBoardIdQuery(int id)
    {
        Id = id;
    }
}