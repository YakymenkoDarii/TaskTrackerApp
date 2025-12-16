using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Features.Boards.Commands.AddNewUser;

internal class AddNewUserCommandHandler : IRequestHandler<AddNewUserCommand, int>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public AddNewUserCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<int> Handle(AddNewUserCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var boardMember = new BoardMember
        {
            UserId = request.UserId,
            BoardId = request.BoardId,
            Role = request.Role,
        };

        var newId = await uow.BoardRepository.AddNewMemberAsync(boardMember);

        await uow.SaveChangesAsync(cancellationToken);

        return newId;
    }
}