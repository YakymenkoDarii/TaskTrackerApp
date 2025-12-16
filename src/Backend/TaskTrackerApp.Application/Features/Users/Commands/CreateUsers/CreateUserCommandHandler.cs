using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Features.Users.Commands.CreateUsers;

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public CreateUserCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var user = new User
        {
            Email = request.Email,
            PasswordHash = request.PasswordHash,
            Tag = request.Tag,
            DisplayName = request.DisplayName,
            CreatedAt = DateTime.UtcNow,
        };

        var result = await uow.UserRepository.AddAsync(user);

        await uow.SaveChangesAsync(cancellationToken);

        return result;
    }
}