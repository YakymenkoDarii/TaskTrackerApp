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

        var memberships = await uow.BoardMembersRepository.GetByUserIdAsync(request.UserId);

        var boardDtos = memberships.Select(m => new BoardDto
        {
            Id = m.Board.Id,
            Title = m.Board.Title,
            Description = m.Board.Description,
            // You might map the user's role here too if needed
            // Role = m.Role.ToString()
        });

        return Result<IEnumerable<BoardDto>>.Success(boardDtos);
    }
}