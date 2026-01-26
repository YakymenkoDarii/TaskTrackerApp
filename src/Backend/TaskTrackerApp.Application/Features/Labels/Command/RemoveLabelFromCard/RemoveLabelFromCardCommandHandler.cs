using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.RemoveLabelFromCard;

public class RemoveLabelFromCardCommandHandler : IRequestHandler<RemoveLabelFromCardCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICardNotifier _cardNotifier;

    public RemoveLabelFromCardCommandHandler(IUnitOfWorkFactory uowFactory, ICardNotifier cardNotifier)
    {
        _uowFactory = uowFactory;
        _cardNotifier = cardNotifier;
    }

    public async Task<Result> Handle(RemoveLabelFromCardCommand request, CancellationToken ct)
    {
        using var uow = _uowFactory.Create();

        var card = await uow.CardRepository.GetCardDetailsAsync(request.CardId);
        if (card == null) return Result.Failure(Error.NotFound);

        var label = card.Labels.FirstOrDefault(l => l.Id == request.LabelId);

        if (label != null)
        {
            card.Labels.Remove(label);
            await uow.SaveChangesAsync(ct);
        }

        await _cardNotifier.NotifyLabelRemovedAsync(card.Id, label.Id);

        return Result.Success();
    }
}