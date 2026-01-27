using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Users.Commands.UpdateUserAvatar;

public class UpdateUserAvatarCommand : IRequest<Result<Uri>>
{
    public Stream FileContent { get; set; }

    public string FileName { get; set; }

    public string ContentType { get; set; }
}