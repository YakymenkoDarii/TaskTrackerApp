using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.UpdateBoardTheme;

internal class UpdateBoardThemeCommandHandler : IRequestHandler<UpdateBoardThemeCommand, Result>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWorkFactory _uowFactory;

    public UpdateBoardThemeCommandHandler(
        ICurrentUserService currentUserService,
        IUnitOfWorkFactory uowFactory)
    {
        _currentUserService = currentUserService;
        _uowFactory = uowFactory;
    }

    public async Task<Result> Handle(UpdateBoardThemeCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return UserErrors.Unauthorized;

        using var uow = _uowFactory.Create();

        var boardMember = await uow.BoardMembersRepository.GetMemberAsync(request.BoardId, userId.Value);
        boardMember.ThemeColor = request.NewColor;
        await uow.SaveChangesAsync();

        return Result.Success();
    }
}