using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.AddLabelToCard;

public class AddLabelToCardCommandHandler : IRequestHandler<AddLabelToCardCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICardNotifier _cardNotifier;

    public AddLabelToCardCommandHandler(IUnitOfWorkFactory uowFactory, ICardNotifier cardNotifier)
    {
        _uowFactory = uowFactory;
        _cardNotifier = cardNotifier;
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

        await _cardNotifier.NotifyLabelAddedAsync(card.Id, label.Id);

        return Result.Success();
    }
}