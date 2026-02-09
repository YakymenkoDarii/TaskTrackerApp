using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Faq.Commands;

public class SeedFaqDataCommand : IRequest<Result<string>>
{
}