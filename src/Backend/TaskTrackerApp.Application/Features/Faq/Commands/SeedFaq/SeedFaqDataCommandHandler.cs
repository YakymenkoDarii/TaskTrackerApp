using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Faq.Commands.SeedFaq;

public class SeedFaqDataCommandHandler : IRequestHandler<SeedFaqDataCommand, Result<string>>
{
    private readonly IFaqService _faqService;

    public SeedFaqDataCommandHandler(IFaqService faqService)
    {
        _faqService = faqService;
    }

    public async Task<Result<string>> Handle(SeedFaqDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _faqService.IngestDummyDataAsync();

            return Result<string>.Success("Dummy sensitive data has been successfully ingested into Azure AI Search.");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(new Error("SeedError", $"Failed to seed data: {ex.Message}"));
        }
    }
}