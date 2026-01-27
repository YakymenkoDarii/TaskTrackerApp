using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Users.Commands.UpdateUsers;

internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var userId = _currentUserService.UserId;
        if (userId is null)
        {
            return Result.Failure(UserErrors.Unauthorized);
        }

        var user = await uow.UserRepository.GetById(userId.Value);
        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        if (user.Tag != request.Tag)
        {
            var userWithTag = await uow.UserRepository.GetByTagAsync(request.Tag);
            Console.WriteLine("WE FOUND TAG");
            if (userWithTag != null && userWithTag.Id != user.Id)
            {
                Console.WriteLine("ERROR");
                return Result.Failure(UserErrors.TagInUse);
            }
        }

        user.Tag = request.Tag;
        user.DisplayName = request.DisplayName;

        await uow.UserRepository.UpdateAsync(user);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}