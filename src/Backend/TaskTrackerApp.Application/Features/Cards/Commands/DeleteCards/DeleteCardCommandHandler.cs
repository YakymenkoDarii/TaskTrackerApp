using MediatR;
using TaskTrackerApp.Application.Features.Cards.Commands.DeleteCard;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Cards.Commands.DeleteCards;

public class DeleteCardCommandHandler : IRequestHandler<DeleteCardCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public DeleteCardCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task Handle(DeleteCardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        await uow.CardRepository.DeleteAsync(request.Id);

        await uow.SaveChangesAsync(cancellationToken);
    }
}