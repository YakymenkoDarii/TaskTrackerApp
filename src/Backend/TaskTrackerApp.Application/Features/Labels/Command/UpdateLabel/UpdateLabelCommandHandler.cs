using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.LabelMappers;
using TaskTrackerApp.Domain.DTOs.Labels;
using TaskTrackerApp.Domain.Errors.Auth;
using TaskTrackerApp.Domain.Errors.Label;
using TaskTrackerApp.Domain.Events.Labels;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.UpdateLabel;

public class UpdateLabelCommandHandler : IRequestHandler<UpdateLabelCommand, Result<LabelDto>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBoardNotifier _boardNotifier;

    public UpdateLabelCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService, IBoardNotifier boardNotifier)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
        _boardNotifier = boardNotifier;
    }

    public async Task<Result<LabelDto>> Handle(UpdateLabelCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var currentUserId = _currentUserService.UserId;
        if (currentUserId == 0)
        {
            return Result<LabelDto>.Failure(AuthErrors.NotAuthenticated);
        }

        var label = await uow.LabelsRepository.GetById(request.Dto.Id);

        if (label == null)
        {
            return Result<LabelDto>.Failure(LabelErrors.NotFound);
        }

        label.UpdatedById = currentUserId.Value;

        label.ApplyUpdate(request.Dto);

        await uow.SaveChangesAsync(cancellationToken);

        var evt = new LabelUpdatedEvent(label.BoardId, label.Id, label.Name, label.Color);
        await _boardNotifier.NotifyLabelUpdatedAsync(evt);

        return label.ToDto();
    }
}