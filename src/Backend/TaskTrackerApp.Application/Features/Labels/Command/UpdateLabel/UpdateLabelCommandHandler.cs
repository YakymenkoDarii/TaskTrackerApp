using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.LabelMappers;
using TaskTrackerApp.Domain.DTOs.Labels;
using TaskTrackerApp.Domain.Errors.Auth;
using TaskTrackerApp.Domain.Errors.Label;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.UpdateLabel;

public class UpdateLabelCommandHandler : IRequestHandler<UpdateLabelCommand, Result<LabelDto>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public UpdateLabelCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
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

        return label.ToDto();
    }
}