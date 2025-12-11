using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Features.Boards.Commands.DeleteBoards;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Boards.Commands.UpdateBoards;
public class UpdateBoardCommandHandler : IRequestHandler<UpdateBoardCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public UpdateBoardCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }
    public async Task Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var board = await uow.BoardRepository.GetAsync(request.Id);

        board.Title = request.Title;
        board.Description = request.Description;
        board.UpdatedBy = request.UpdatedBy;
        board.UpdatedAt = DateTime.UtcNow;

        await uow.BoardRepository.UpdateAsync(board);

        await uow.SaveChangesAsync(cancellationToken);
    }
}
