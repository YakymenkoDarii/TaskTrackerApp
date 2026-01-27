using MediatR;
using TaskTrackerApp.Application.Interfaces.BlobStorage;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Constants;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Users.Commands.UpdateUserAvatar;

public class UpdateUserAvatarCommandHandler : IRequestHandler<UpdateUserAvatarCommand, Result<Uri>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBlobStorageService _blob;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserAvatarCommandHandler(IUnitOfWorkFactory uowFactory,
        IBlobStorageService blob,
        ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _blob = blob;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Uri>> Handle(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        if (_currentUserService.UserId is not { } userId)
        {
            return Result<Uri>.Failure(UserErrors.Unauthorized);
        }

        var user = await uow.UserRepository.GetById(userId);

        if (user == null)
        {
            return Result<Uri>.Failure(UserErrors.NotFound);
        }

        var oldAvatarUrl = user.AvatarUrl;

        var fileExtension = Path.GetExtension(request.FileName);
        var blobName = $"avatar-{userId}{fileExtension}";

        var avatarUrl = await _blob.UploadAsync(
            request.FileContent,
            BlobContainerNames.UserAvatars,
            blobName,
            request.ContentType
        );

        user.AvatarUrl = avatarUrl;
        await uow.SaveChangesAsync();

        if (!string.IsNullOrEmpty(oldAvatarUrl) && oldAvatarUrl != avatarUrl)
        {
            var oldBlobName = Path.GetFileName(new Uri(oldAvatarUrl).LocalPath);
            await _blob.DeleteAsync(BlobContainerNames.UserAvatars, oldBlobName);
        }

        return new Uri(avatarUrl);
    }
}