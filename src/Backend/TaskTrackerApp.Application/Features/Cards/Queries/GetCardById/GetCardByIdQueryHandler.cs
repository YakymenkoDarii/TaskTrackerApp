using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.CardMappers;
using TaskTrackerApp.Domain.DTOs.Card;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetCardById;

public class GetCardByIdQueryHandler : IRequestHandler<GetCardByIdQuery, CardDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public GetCardByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<CardDto> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.Create();

        var card = await uow.CardRepository.GetCardDetailsAsync(request.Id);

        if (card == null)
        {
            return null;
        }

        return card.ToDto();
    }
}