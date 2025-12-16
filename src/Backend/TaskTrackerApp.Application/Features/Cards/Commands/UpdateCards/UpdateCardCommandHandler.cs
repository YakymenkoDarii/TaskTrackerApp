using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Cards.Commands.UpdateCards;

internal class UpdateCardCommandHandler : IRequestHandler<UpdateCardCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public UpdateCardCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task Handle(UpdateCardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var card = await uow.CardRepository.GetAsync(request.Id);

        card.ColumnId = request.ColumnId;
        card.Title = request.Title;
        card.Description = request.Description;
        card.DueDate = request.DueDate;
        card.AssigneeId = request.AssigneeId;
        card.UpdatedById = request.UpdatedById;
        card.UpdatedAt = DateTime.UtcNow;

        await uow.CardRepository.UpdateAsync(card);

        await uow.SaveChangesAsync(cancellationToken);
    }
}