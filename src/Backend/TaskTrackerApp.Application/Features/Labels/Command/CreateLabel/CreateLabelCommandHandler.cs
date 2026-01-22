using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.LabelMappers;
using TaskTrackerApp.Domain.DTOs.Labels;
using TaskTrackerApp.Domain.Errors.Auth;
using TaskTrackerApp.Domain.Events.Labels;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.CreateLabel;

public class CreateLabelCommandHandler : IRequestHandler<CreateLabelCommand, Result<LabelDto>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBoardNotifier _boardNotifier;

    public CreateLabelCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService, IBoardNotifier boardNotifier)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
        _boardNotifier = boardNotifier;
    }

    public async Task<Result<LabelDto>> Handle(CreateLabelCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var currentUserId = _currentUserService.UserId;
        if (currentUserId == 0)
        {
            return Result<LabelDto>.Failure(AuthErrors.NotAuthenticated);
        }

        var labelEntity = request.CreateLabel.ToEntity();

        labelEntity.CreatedById = currentUserId.Value;
        labelEntity.UpdatedById = currentUserId.Value;

        await uow.LabelsRepository.AddAsync(labelEntity);

        await uow.SaveChangesAsync(cancellationToken);

        var resultDto = labelEntity.ToDto();

        var evt = new LabelCreatedEvent(labelEntity.BoardId, labelEntity.Id, labelEntity.Name, labelEntity.Color);

        await _boardNotifier.NotifyLabelCreatedAsync(evt);

        return Result<LabelDto>.Success(resultDto);
    }
}