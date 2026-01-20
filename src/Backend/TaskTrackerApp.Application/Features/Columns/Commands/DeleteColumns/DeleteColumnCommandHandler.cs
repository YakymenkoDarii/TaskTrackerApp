using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Events.Column;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Columns.Commands.DeleteColumns;

internal class DeleteColumnCommandHandler : IRequestHandler<DeleteColumnCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBoardNotifier _notifier;

    public DeleteColumnCommandHandler(
        IUnitOfWorkFactory uowFactory,
        IBoardNotifier notifier)
    {
        _uowFactory = uowFactory;
        _notifier = notifier;
    }

    public async Task<Result> Handle(DeleteColumnCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var column = await uow.ColumnRepository.GetById(request.Id);
        if (column == null)
        {
            return Result.Success();
        }

        int boardId = column.BoardId;

        await uow.ColumnRepository.DeleteAsync(request.Id);
        await uow.SaveChangesAsync(cancellationToken);

        var evt = new ColumnDeletedEvent(request.Id, boardId);

        _ = _notifier.NotifyColumnDeletedAsync(evt);

        return Result.Success();
    }
}