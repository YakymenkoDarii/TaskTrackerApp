using MediatR;
using TaskTrackerApp.Application.Features.Behaviors.Interfaces.Boards;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Behaviors.Board;

public class BoardActivityBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBoardRelatedCommand
    where TResponse : Result
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public BoardActivityBehavior(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        if (response.IsSuccess)
        {
            try
            {
                using var uow = _uowFactory.Create();

                var board = await uow.BoardRepository.GetById(request.BoardId);

                if (board != null)
                {
                    board.LastModified = DateTime.UtcNow;

                    await uow.BoardRepository.UpdateAsync(board);
                    await uow.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Warning] Failed to update LastModified for Board {request.BoardId}: {ex.Message}");
            }
        }

        return response;
    }
}