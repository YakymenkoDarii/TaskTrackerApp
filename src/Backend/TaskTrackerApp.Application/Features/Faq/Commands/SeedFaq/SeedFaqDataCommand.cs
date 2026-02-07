using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Faq.Commands;

public class SeedFaqDataCommand : IRequest<Result<string>>
{
}