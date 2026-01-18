using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.AddLabelToCard;

public class AddLabelToCardCommandHandler : IRequestHandler<AddLabelToCardCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public AddLabelToCardCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result> Handle(AddLabelToCardCommand request, CancellationToken ct)
    {
        using var uow = _uowFactory.Create();

        var card = await uow.CardRepository.GetCardDetailsAsync(request.CardId);
        if (card == null) return Result.Failure(Error.NotFound);

        if (card.Labels.Any(l => l.Id == request.LabelId))
            return Result.Success();

        var label = await uow.LabelsRepository.GetById(request.LabelId);
        if (label == null) return Result.Failure(Error.NotFound);

        card.Labels.Add(label);
        await uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}