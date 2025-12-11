using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Boards.Commands.DeleteBoards;

public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public DeleteBoardCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        await uow.BoardRepository.DeleteAsync(request.Id);

        await uow.SaveChangesAsync(cancellationToken);
    }
}