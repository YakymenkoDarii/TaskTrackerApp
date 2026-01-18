using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.CardMappers;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetByBoardId;

public class GetCardsByBoardIdQueryHandler : IRequestHandler<GetCardsByBoardIdQuery, Result<IEnumerable<CardDto>>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetCardsByBoardIdQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<CardDto>>> Handle(GetCardsByBoardIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var cards = uow.CardRepository.GetQueryable();
        cards = cards.Where(c => c.BoardId == request.BoardId);

        var dtos = cards.Select(c => c.ToDto());

        return dtos.ToList();
    }
}