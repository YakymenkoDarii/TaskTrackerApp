using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<Result>
{
    public string OldPassword { get; set; }

    public string NewPassword { get; set; }

    public string NewPasswordConfirm { get; set; }
}