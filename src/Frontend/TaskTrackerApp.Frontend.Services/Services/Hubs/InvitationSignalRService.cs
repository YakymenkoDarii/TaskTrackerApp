using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using TaskTrackerApp.Domain.Events.Invitations;
using TaskTrackerApp.Frontend.Domain.Constants;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Hubs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Hubs;

public class InvitationSignalRService : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly NavigationManager _navigationManager;
    private readonly ITokenStorage _tokenStorage;

    public event Action<InvitationReceivedEvent>? OnInviteReceived;

    public event Action<int>? OnInviteRevoked;

    public event Action<InvitationRespondedEvent>? OnInviteResponded;

    public InvitationSignalRService(
        NavigationManager navigationManager,
        ITokenStorage tokenStorage,
        IConfiguration configuration)
    {
        _navigationManager = navigationManager;
        _tokenStorage = tokenStorage;

        var apiBaseUrl = configuration["BaseUri"];

        var hubUrl = $"{apiBaseUrl}{HubRoutes.Invitations}";

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () =>
                {
                    var token = tokenStorage.GetAccessToken();
                    return Task.FromResult(token);
                };
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<InvitationReceivedEvent>(nameof(IInvitationClient.ReceiveInvite), (notification) =>
        {
            OnInviteReceived?.Invoke(notification);
        });

        _hubConnection.On<int>(nameof(IInvitationClient.RevokeInvite), (invitationId) =>
        {
            OnInviteRevoked?.Invoke(invitationId);
        });

        _hubConnection.On<InvitationRespondedEvent>(nameof(IInvitationClient.InviteResponded), (notification) =>
        {
            OnInviteResponded?.Invoke(notification);
        });
    }

    public async Task StartConnection()
    {
        if (_hubConnection.State != HubConnectionState.Disconnected)
        {
            return;
        }

        var token = _tokenStorage.GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            return;
        }

        try
        {
            await _hubConnection.StartAsync();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("SignalR Unauthorized: User likely logged out.");
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