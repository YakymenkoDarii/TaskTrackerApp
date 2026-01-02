using MediatR;
using TaskTrackerApp.Domain.DTOs.Auth.Responses;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Auth.Commands.RefreshTokenCommand;

public class RefreshTokenCommand : IRequest<Result<LoginResponse>>
{
    public string RefreshToken { get; set; } = default!;
    public string Tag { get; set; } = default!;
}