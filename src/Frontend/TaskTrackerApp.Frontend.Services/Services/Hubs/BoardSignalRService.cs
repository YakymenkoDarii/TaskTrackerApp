using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.Constants;
using TaskTrackerApp.Frontend.Domain.Events.BoardMember;
using TaskTrackerApp.Frontend.Domain.Events.Card;
using TaskTrackerApp.Frontend.Domain.Events.Column;
using TaskTrackerApp.Frontend.Domain.Events.Invitations;
using TaskTrackerApp.Frontend.Domain.Events.Labels;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Hubs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Hubs;

public class BoardSignalRService : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly ITokenStorage _tokenStorage;

    // Columns
    public event Action<ColumnCreatedEvent>? OnColumnCreated;

    public event Action<ColumnMovedEvent>? OnColumnMoved;

    public event Action<ColumnDeletedEvent>? OnColumnDeleted;

    // Cards
    public event Action<CardCreatedEvent>? OnCardCreated;

    public event Action<CardMovedEvent>? OnCardMoved;

    public event Action<CardUpdatedEvent>? OnCardUpdated;

    public event Action<CardDeletedEvent>? OnCardDeleted;

    // Members
    public event Action<BoardMemberAddedEvent>? OnMemberAdded;

    public event Action<BoardMemberRemovedEvent>? OnMemberRemoved;

    public event Action<BoardMemberRoleUpdatedEvent>? OnMemberRoleUpdated;

    // Invitations
    public event Action<BoardInvitationAddedEvent>? OnInvitationAdded;

    public event Action<BoardInvitationRevokedEvent>? OnInvitationRevoked;

    // Labels
    public event Action<LabelCreatedEvent>? OnLabelCreated;

    public event Action<LabelUpdatedEvent>? OnLabelUpdated;

    public event Action<LabelDeletedEvent>? OnLabelDeleted;

    public BoardSignalRService(
        NavigationManager navigationManager,
        ITokenStorage tokenStorage,
        IConfiguration configuration)
    {
        _tokenStorage = tokenStorage;
        var apiBaseUrl = configuration["BaseUri"];
        var hubUrl = $"{apiBaseUrl}{HubRoutes.Board}";

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
        // Columns
        _hubConnection.On<ColumnCreatedEvent>(nameof(IBoardClient.ColumnCreated), e => OnColumnCreated?.Invoke(e));
        _hubConnection.On<ColumnMovedEvent>(nameof(IBoardClient.ColumnMoved), e => OnColumnMoved?.Invoke(e));
        _hubConnection.On<ColumnDeletedEvent>(nameof(IBoardClient.ColumnDeleted), e => OnColumnDeleted?.Invoke(e));

        // Cards
        _hubConnection.On<CardCreatedEvent>(nameof(IBoardClient.CardCreated), e => OnCardCreated?.Invoke(e));
        _hubConnection.On<CardMovedEvent>(nameof(IBoardClient.CardMoved), e => OnCardMoved?.Invoke(e));
        _hubConnection.On<CardUpdatedEvent>(nameof(IBoardClient.CardUpdated), e => OnCardUpdated?.Invoke(e));
        _hubConnection.On<CardDeletedEvent>(nameof(IBoardClient.CardDeleted), e => OnCardDeleted?.Invoke(e));

        // Members
        _hubConnection.On<BoardMemberAddedEvent>(nameof(IBoardClient.MemberAdded), e => OnMemberAdded?.Invoke(e));
        _hubConnection.On<BoardMemberRemovedEvent>(nameof(IBoardClient.MemberRemoved), e => OnMemberRemoved?.Invoke(e));
        _hubConnection.On<BoardMemberRoleUpdatedEvent>(nameof(IBoardClient.MemberRoleUpdated), e => OnMemberRoleUpdated?.Invoke(e));

        // Invitations
        _hubConnection.On<BoardInvitationAddedEvent>(nameof(IBoardClient.InvitationAdded), e => OnInvitationAdded?.Invoke(e));
        _hubConnection.On<BoardInvitationRevokedEvent>(nameof(IBoardClient.InvitationRevoked), e => OnInvitationRevoked?.Invoke(e));

        // Labels
        _hubConnection.On<LabelCreatedEvent>(nameof(IBoardClient.LabelCreated), e => OnLabelCreated?.Invoke(e));
        _hubConnection.On<LabelUpdatedEvent>(nameof(IBoardClient.LabelUpdated), e => OnLabelUpdated?.Invoke(e));
        _hubConnection.On<LabelDeletedEvent>(nameof(IBoardClient.LabelDeleted), e => OnLabelDeleted?.Invoke(e));
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
                Console.WriteLine($"Error starting Board Hub: {ex.Message}");
            }
        }
    }

    public async Task JoinBoard(int boardId)
    {
        int retries = 0;
        while (_hubConnection.State != HubConnectionState.Connected && retries < 20)
        {
            await Task.Delay(50);
            retries++;
        }
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            Console.WriteLine($"[SignalR] Requesting to join group: Board_{boardId}");
            await _hubConnection.InvokeAsync("JoinBoardGroup", boardId);
        }
        else
        {
            Console.WriteLine($"[SignalR FAILURE] Could not join board {boardId}. State: {_hubConnection.State}");
        }
    }

    public async Task LeaveBoard(int boardId)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("LeaveBoardGroup", boardId);
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