using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors.Label;
using TaskTrackerApp.Domain.Events.Labels;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.DeleteLabel;

public class DeleteLabelCommandHandler : IRequestHandler<DeleteLabelCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBoardNotifier _boardNotifier;

    public DeleteLabelCommandHandler(IUnitOfWorkFactory uowFactory, IBoardNotifier boardNotifier)
    {
        _uowFactory = uowFactory;
        _boardNotifier = boardNotifier;
    }

    public async Task<Result> Handle(DeleteLabelCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var label = await uow.LabelsRepository.GetById(request.LabelId);
        if (label == null)
        {
            return Result.Failure(LabelErrors.NotFound);
        }

        await uow.LabelsRepository.DeleteWithLinksAsync(request.LabelId);
        await uow.SaveChangesAsync(cancellationToken);

        var evt = new LabelDeletedEvent(label.BoardId, label.Id);
        await _boardNotifier.NotifyLabelDeletedAsync(evt);

        return Result.Success();
    }
}