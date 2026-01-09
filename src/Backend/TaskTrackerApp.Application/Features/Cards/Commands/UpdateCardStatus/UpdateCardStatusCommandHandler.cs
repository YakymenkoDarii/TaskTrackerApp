using MediatR;
using TaskTrackerApp.Application.Features.Cards.Commands.UpdateStatusCards;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Commands.UpdateCardStatus;

public class UpdateCardStatusCommandHandler : IRequestHandler<UpdateCardStatusCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public UpdateCardStatusCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result> Handle(UpdateCardStatusCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        await uow.CardRepository.UpdateCardStatus(request.Id, request.IsCompleted);

        await uow.SaveChangesAsync();

        return Result.Success();
    }
}