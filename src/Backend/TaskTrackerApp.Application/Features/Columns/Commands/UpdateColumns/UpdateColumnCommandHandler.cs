using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Events.Column;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Columns.Commands.UpdateColumns;

internal class UpdateColumnCommandHandler : IRequestHandler<UpdateColumnCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBoardNotifier _notifier;

    public UpdateColumnCommandHandler(IUnitOfWorkFactory uowFactory, IBoardNotifier notifier)
    {
        _uowFactory = uowFactory;
        _notifier = notifier;
    }

    public async Task<Result> Handle(UpdateColumnCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var column = await uow.ColumnRepository.GetById(request.Id);

        if (column.Position != request.Position)
        {
            var allColumns = (await uow.ColumnRepository.GetColumnsByBoardIdAsync(request.BoardId)).ToList();

            if (column == null)
            {
                return Result.Failure(new Error("Column.Error", "No column found"));
            }

            int oldPosition = column.Position;
            int newPosition = request.Position;

            request.Position = newPosition;

            if (oldPosition != newPosition)
            {
                if (newPosition < oldPosition)
                {
                    var affectedColumns = allColumns
                        .Where(c => c.Position >= newPosition && c.Position < oldPosition && c.Id != column.Id);

                    foreach (var col in affectedColumns)
                    {
                        col.Position++;
                    }
                }
                else if (newPosition > oldPosition)
                {
                    var affectedColumns = allColumns
                        .Where(c => c.Position > oldPosition && c.Position <= newPosition && c.Id != column.Id);

                    foreach (var col in affectedColumns)
                    {
                        col.Position--;
                    }
                }
            }
        }

        column.Title = request.Title;
        column.Description = request.Description;
        column.BoardId = request.BoardId;
        column.UpdatedAt = DateTime.UtcNow;
        column.Position = request.Position;

        await uow.ColumnRepository.UpdateAsync(column);
        await uow.SaveChangesAsync(cancellationToken);

        var moveEvt = new ColumnMovedEvent(column.Id, column.BoardId, column.Position);
        _ = _notifier.NotifyColumnMovedAsync(moveEvt);

        return Result.Success();
    }
}