using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Boards.Commands.UpdateBoards;

internal class UpdateBoardCommandHandler : IRequestHandler<UpdateBoardCommand>
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
        board.UpdatedById = request.UpdatedById;
        board.UpdatedAt = DateTime.UtcNow;

        await uow.BoardRepository.UpdateAsync(board);

        await uow.SaveChangesAsync(cancellationToken);
    }
}