using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.CardCommentsMappers;
using TaskTrackerApp.Domain.DTOs.CardComment;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.CardComments.Queries;

public class GetCommentsByCardIdQueryHandler : IRequestHandler<GetCommentsByCardIdQuery, Result<IEnumerable<CardCommentDto>>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetCommentsByCardIdQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<CardCommentDto>>> Handle(GetCommentsByCardIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var cardComments = await uow.CardCommentsRepository.GetByCardIdAsync(request.CardId);

        return cardComments.Select(c => c.ToDto()).ToList();
    }
}