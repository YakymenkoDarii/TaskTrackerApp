using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Events.Column;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Columns.Commands.CreateColumns;

internal class CreateColumnCommandHandler : IRequestHandler<CreateColumnCommand, Result<int>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBoardNotifier _notifier;

    public CreateColumnCommandHandler(IUnitOfWorkFactory uowFactory, IBoardNotifier notifier)
    {
        _uowFactory = uowFactory;
        _notifier = notifier;
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

        var evt = new ColumnCreatedEvent(column.Id, column.BoardId, column.Title);
        _ = _notifier.NotifyColumnCreatedAsync(evt);

        return column.Id;
    }
}