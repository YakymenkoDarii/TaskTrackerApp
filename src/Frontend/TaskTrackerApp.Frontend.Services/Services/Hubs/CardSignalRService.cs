using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.Constants;
using TaskTrackerApp.Frontend.Domain.Events.Comment;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Hubs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Hubs;

public class CardSignalRService : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly ITokenStorage _tokenStorage;

    public event Action<CommentAddedEvent>? OnCommentAdded;

    public event Action<CommentUpdatedEvent>? OnCommentUpdated;

    public event Action<int>? OnCommentDeleted;

    public event Action<int, int>? OnLabelAdded;

    public event Action<int, int>? OnLabelRemoved;

    public CardSignalRService(
        NavigationManager navigationManager,
        ITokenStorage tokenStorage,
        IConfiguration configuration)
    {
        _tokenStorage = tokenStorage;
        var apiBaseUrl = configuration["BaseUri"];
        var hubUrl = $"{apiBaseUrl}{HubRoutes.Card}";

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(_tokenStorage.GetAccessToken());
            })
            .WithAutomaticReconnect()
            .Build();

        RegisterHandlers();
    }

    private void RegisterHandlers()
    {
        // Comments
        _hubConnection.On<CommentAddedEvent>(nameof(ICardClient.CommentAdded), e => OnCommentAdded?.Invoke(e));
        _hubConnection.On<CommentUpdatedEvent>(nameof(ICardClient.CommentUpdated), e => OnCommentUpdated?.Invoke(e));
        _hubConnection.On<int>(nameof(ICardClient.CommentDeleted), (commentId) => OnCommentDeleted?.Invoke(commentId));

        // Label\
        _hubConnection.On<int, int>(nameof(ICardClient.LabelAdded), (cardId, labelId) => OnLabelAdded?.Invoke(cardId, labelId));
        _hubConnection.On<int, int>(nameof(ICardClient.LabelRemoved), (cardId, labelId) => OnLabelRemoved?.Invoke(cardId, labelId));
    }

    public async Task StartConnection()
    {
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting Card Hub: {ex.Message}");
            }
        }
    }

    public async Task JoinCard(int cardId)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("JoinCardGroup", cardId);
        }
    }

    public async Task LeaveCard(int cardId)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("LeaveCardGroup", cardId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}