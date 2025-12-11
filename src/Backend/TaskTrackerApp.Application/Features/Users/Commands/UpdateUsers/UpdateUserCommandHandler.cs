using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Features.Columns.Commands.UpdateColumns;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Users.Commands.UpdateUsers;
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public UpdateUserCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var user = await uow.UserRepository.GetAsync(request.Id);

        user.Tag = request.Tag;
        user.PasswordHash = request.PasswordHash;
        user.DisplayName = request.DisplayName;
        user.AvatarUrl = request.AvatarUrl;

        await uow.UserRepository.UpdateAsync(user);

        await uow.SaveChangesAsync(cancellationToken);
    }
}
