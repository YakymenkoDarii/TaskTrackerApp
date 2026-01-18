using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Errors.Label;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.DeleteLabel;

public class DeleteLabelCommandHandler : IRequestHandler<DeleteLabelCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public DeleteLabelCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
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

        return Result.Success();
    }
}