using MediatR;
using TaskTrackerApp.Domain.DTOs.Card;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetCardById;

public class GetCardByIdQuery : IRequest<CardDto>
{
    public int Id { get; set; }

    public GetCardByIdQuery(int id)
    {
        Id = id;
    }
}