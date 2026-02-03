using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Boards.Commands.MarkAsBackedUp;

public class MarkBoardAsBackedUpCommandHandler : IRequestHandler<MarkBoardAsBackedUpCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public MarkBoardAsBackedUpCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task Handle(MarkBoardAsBackedUpCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var board = await uow.BoardRepository.GetById(request.BoardId);

        if (board != null)
        {
            board.IsBackedUp = true;

            uow.BoardRepository.UpdateAsync(board);
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}