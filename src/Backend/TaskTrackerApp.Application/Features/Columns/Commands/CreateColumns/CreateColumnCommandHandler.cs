using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Columns.Commands.CreateColumns;

internal class CreateColumnCommandHandler : IRequestHandler<CreateColumnCommand, Result<int>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public CreateColumnCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<int>> Handle(CreateColumnCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var columns = await uow.ColumnRepository.GetColumnsByBoardIdAsync(request.BoardId);
        int newLastPosition;

        if (columns.Any())
        {
            newLastPosition = columns.Max(c => c.Position) + 1;
        }
        else
        {
            newLastPosition = 0;
        }
        var column = new Column
        {
            Title = request.Title,
            Description = request.Description,
            BoardId = request.BoardId,
            Position = newLastPosition
        };

        await uow.ColumnRepository.AddAsync(column);
        await uow.SaveChangesAsync(cancellationToken);

        return column.Id;
    }
}