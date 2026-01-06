using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetBoardById;

public class GetBoardByIdQueryHandler : IRequestHandler<GetBoardByIdQuery, Result<BoardDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public GetBoardByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Result<BoardDto>> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetAsync(request.Id);

        if (board == null)
        {
            return null;
        }

        return new BoardDto
        {
            Id = board.Id,
            Title = board.Title,
            Description = board.Description,
        };
    }
}