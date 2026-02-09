using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;
using TaskTrackerApp.Frontend.Domain.DTOs.ChatMessages;

namespace TaskTrackerApp.Application.Features.Faq.Commands.AskQuestion;

public class AskQuestionCommandHandler : IRequestHandler<AskQuestionCommand, Result<ChatResponse>>
{
    private readonly IFaqService _faqService;
    private readonly IChatHistoryService _history;

    public AskQuestionCommandHandler(IFaqService faqService, IChatHistoryService history)
    {
        _faqService = faqService;
        _history = history;
    }

    public async Task<Result<ChatResponse>> Handle(AskQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _history.AddMessageAsync(request.SessionId, "User", request.Question);

            var answerText = await _faqService.AskQuestionAsync(request.Question);

            await _history.AddMessageAsync(request.SessionId, "AI", answerText);

            return Result<ChatResponse>.Success(new ChatResponse(answerText));
        }
        catch (Exception ex)
        {
            return Result<ChatResponse>.Failure(new Error("AiError", ex.Message));
        }
    }
}