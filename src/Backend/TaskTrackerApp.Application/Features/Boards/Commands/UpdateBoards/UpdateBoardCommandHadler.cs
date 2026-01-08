using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.UpdateBoards;

internal class UpdateBoardCommandHandler : IRequestHandler<UpdateBoardCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public UpdateBoardCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result> Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var board = await uow.BoardRepository.GetAsync(request.Id);

        if (board == null)
        {
            return Result.Failure(new Error("Board.NotFound", "Board not found"));
        }
        board.Title = request.Title;
        board.Description = request.Description;
        board.UpdatedById = request.UpdatedById;
        board.UpdatedAt = DateTime.UtcNow;

        await uow.BoardRepository.UpdateAsync(board);

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}