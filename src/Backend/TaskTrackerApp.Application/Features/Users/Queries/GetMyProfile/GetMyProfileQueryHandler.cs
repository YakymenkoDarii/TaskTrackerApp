using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.User;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Users.Queries.GetMyProfile;

internal class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<MyProfileDto>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public GetMyProfileQueryHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<MyProfileDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<MyProfileDto>.Failure(UserErrors.Unauthorized);
        }

        var user = await uow.UserRepository.GetById(userId.Value);

        if (user == null)
        {
            return Result<MyProfileDto>.Failure(UserErrors.NotFound);
        }

        var dto = new MyProfileDto
        {
            Tag = user.Tag,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl,
        };

        return Result<MyProfileDto>.Success(dto);
    }
}