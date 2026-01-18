using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.LabelMappers;
using TaskTrackerApp.Domain.DTOs.Labels;
using TaskTrackerApp.Domain.Errors.Auth;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.CreateLabel;

public class CreateLabelCommandHandler : IRequestHandler<CreateLabelCommand, Result<LabelDto>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public CreateLabelCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
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

        return Result<LabelDto>.Success(resultDto);
    }
}