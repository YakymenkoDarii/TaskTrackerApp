using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TaskTrackerApp.Frontend.Domain.DTOs.ChatMessages;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Components.Chat;

public partial class AiChatWindow
{
    [Inject] private IChatService ChatService { get; set; }

    [Inject] private ILocalStorageService LocalStorage { get; set; }

    [Parameter] public bool Open { get; set; }

    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    private List<ChatMessageDto> _messages = new();
    private string _inputMessage;
    private bool _isThinking;
    private bool _isLoadingHistory;
    private string _sessionId;

    protected override async Task OnInitializedAsync()
    {
        _sessionId = await LocalStorage.GetItemAsync<string>("chatSessionId");
        if (string.IsNullOrEmpty(_sessionId))
        {
            _sessionId = Guid.NewGuid().ToString();
            await LocalStorage.SetItemAsync("chatSessionId", _sessionId);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Open && !_messages.Any())
        {
            Console.WriteLine($"[DEBUG] Loading history for session: {_sessionId}");
            _isLoadingHistory = true;

            var result = await ChatService.GetHistoryAsync(_sessionId);

            Console.WriteLine($"[DEBUG] API Success: {result.IsSuccess}");

            if (result.IsSuccess)
            {
                Console.WriteLine($"[DEBUG] Message Count: {result.Value?.Count() ?? 0}");
                if (result.Value != null)
                {
                    foreach (var msg in result.Value)
                    {
                        Console.WriteLine($"[DEBUG] Msg: {msg.Role} - {msg.Content}");
                    }
                    _messages = result.Value.ToList();
                }
            }
            else
            {
                Console.WriteLine($"[DEBUG] Error: {result.Error.Message}");
            }

            _isLoadingHistory = false;
            StateHasChanged();
        }
    }

    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(_inputMessage)) return;

        var currentQ = _inputMessage;
        _inputMessage = string.Empty;

        _messages.Add(new ChatMessageDto { Role = "User", Content = currentQ });
        _isThinking = true;
        StateHasChanged();

        var request = new ChatRequest(currentQ, _sessionId);
        var result = await ChatService.AskAsync(request);

        if (result.IsSuccess)
        {
            _messages.Add(new ChatMessageDto { Role = "AI", Content = result.Value.Answer });
        }
        else
        {
            _messages.Add(new ChatMessageDto { Role = "AI", Content = "Sorry, I couldn't reach the server." });
        }

        _isThinking = false;
        StateHasChanged();
    }

    private async Task HandleKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter") await SendMessageAsync();
    }

    private async Task OnClose()
    {
        Open = false;
        await OpenChanged.InvokeAsync(Open);
    }
}