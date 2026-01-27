using MediatR;
using TaskTrackerApp.Domain.DTOs.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Users.Queries.GetMyProfile;

public class GetMyProfileQuery : IRequest<Result<MyProfileDto>>
{
}