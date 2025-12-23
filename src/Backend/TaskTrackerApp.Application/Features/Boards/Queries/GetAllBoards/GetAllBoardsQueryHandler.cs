using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetAllBoards;

public class GetAllBoardsQueryHandler : IRequestHandler<GetAllBoardsQuery, Result<IEnumerable<BoardDto>>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public GetAllBoardsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Result<IEnumerable<BoardDto>>> Handle(GetAllBoardsQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.Create();

        var boards = await uow.BoardRepository.GetAllWithOwnerAsync(request.UserId);

        var boardDtos = boards.Select(b => new BoardDto
        {
            Id = b.Id,
            Title = b.Title,
            Description = b.Description,
            LastTimeOpenned = b.LastTimeOpenned,
        });

        return boardDtos.ToList();
    }
}