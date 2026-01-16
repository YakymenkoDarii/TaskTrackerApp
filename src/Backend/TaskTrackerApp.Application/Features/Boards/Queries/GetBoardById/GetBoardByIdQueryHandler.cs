using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Errors;
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

        var isMember = await uow.BoardMembersRepository.ExistsAsync(request.Id, request.CurrentUserId);

        if (!isMember)
        {
            return Result<BoardDto>.Failure(new Error("Unauthorized", "You do not have access to this board."));
        }

        var board = await uow.BoardRepository.GetById(request.Id);

        if (board == null)
        {
            return Result<BoardDto>.Failure(new Error("NotFound", "Board not found"));
        }

        var dto = new BoardDto
        {
            Id = board.Id,
            Title = board.Title,
            Description = board.Description,
        };

        return Result<BoardDto>.Success(dto);
    }
}