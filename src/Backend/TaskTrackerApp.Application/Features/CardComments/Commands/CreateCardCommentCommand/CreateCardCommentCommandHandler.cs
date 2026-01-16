using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.CardCommentsMappers;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.CardComments.Commands.CreateCardCommentCommand;

public class CreateCardCommentCommandHandler : IRequestHandler<CreateCardCommentCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public CreateCardCommentCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result> Handle(CreateCardCommentCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        await uow.CardCommentsRepository.AddAsync(request!.ToEntity());

        await uow.SaveChangesAsync();

        return Result.Success();
    }
}