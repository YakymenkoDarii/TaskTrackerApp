using MediatR;
using TaskTrackerApp.Application.Features.Behaviors.Interfaces.Boards;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors.Board;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Behaviors.Board;

public class BoardArchivedValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public BoardArchivedValidationBehavior(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IBoardRelatedCommand boardCommand)
        {
            using var uow = _uowFactory.Create();

            var isArchived = await uow.BoardRepository.IsBoardArchivedAsync(boardCommand.BoardId);

            if (isArchived)
            {
                var error = BoardErrors.Archived;

                var failureResult = typeof(Result).IsAssignableFrom(typeof(TResponse))
                    ? (TResponse)(object)Result.Failure(error)
                    : (TResponse)(object)Result<object>.Failure(error);

                return failureResult;
            }
        }
        return await next();
    }
}