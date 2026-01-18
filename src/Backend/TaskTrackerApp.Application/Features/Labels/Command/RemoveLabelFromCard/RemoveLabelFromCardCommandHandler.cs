using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Labels.Command.RemoveLabelFromCard;

public class RemoveLabelFromCardCommandHandler : IRequestHandler<RemoveLabelFromCardCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public RemoveLabelFromCardCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
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

        return Result.Success();
    }
}