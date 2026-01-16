using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Users.Commands.UpdateUsers;

internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public UpdateUserCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var user = await uow.UserRepository.GetById(request.Id);

        user.Tag = request.Tag;
        user.PasswordHash = request.PasswordHash;
        user.DisplayName = request.DisplayName;
        user.AvatarUrl = request.AvatarUrl;

        await uow.UserRepository.UpdateAsync(user);

        await uow.SaveChangesAsync(cancellationToken);
    }
}