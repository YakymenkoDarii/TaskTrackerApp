using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Features.Behaviors.Interfaces.Boards;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Behaviors.Board;

public class BoardLockValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBoardRelatedCommand
    where TResponse : Result
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public BoardLockValidationBehavior(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var board = await uow.BoardRepository.GetById(request.BoardId);

        if (board == null) return await next();

        var owner = await uow.UserRepository.GetById(board.CreatedById);

        if (owner != null && !owner.IsPro)
        {
            var ownerBoards = await uow.BoardRepository.GetByCreatorIdAsync(owner.Id);

            var activeBoardIds = ownerBoards
                .OrderByDescending(b => b.LastModified)
                .Take(3)
                .Select(b => b.Id)
                .ToHashSet();

            if (!activeBoardIds.Contains(board.Id))
            {
                var error = new Error("BoardLocked", "This board is Read-Only because the owner has reached their free plan limit.");

                if (typeof(TResponse) == typeof(Result))
                {
                    return (TResponse)(object)Result.Failure(error);
                }
                else
                {
                    var genericResult = typeof(Result<>)
                        .MakeGenericType(typeof(TResponse).GetGenericArguments()[0])
                        .GetMethod("Failure", new[] { typeof(Error) })
                        .Invoke(null, new object[] { error });

                    return (TResponse)genericResult;
                }
            }
        }

        // 5. If passed, run the command
        return await next();
    }
}