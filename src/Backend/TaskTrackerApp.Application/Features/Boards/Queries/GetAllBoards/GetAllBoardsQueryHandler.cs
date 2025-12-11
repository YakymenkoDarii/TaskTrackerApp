using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Features.Cards.Queries.GetCardById;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.DTOs.Card;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetAllBoards;
public class GetAllBoardsQueryHandler : IRequestHandler<GetAllBoardsQuery, IEnumerable<BoardDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public GetAllBoardsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<IEnumerable<BoardDto>> Handle(GetAllBoardsQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.Create();

        var boards = await uow.BoardRepository.GetAllWithOwnerAsync(request.UserId);

        var boardDtos = boards.Select(b => new BoardDto
        {
            Id = b.Id,
            Title = b.Title,
            Description = b.Description,
            CreatedAt = b.CreatedAt,
            CreatedBy = b.CreatedBy,
        });

        return boardDtos;
    }
}
