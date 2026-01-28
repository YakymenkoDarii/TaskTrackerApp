using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Auth;
using TaskTrackerApp.Frontend.Services.Services.Hubs;

namespace TaskTrackerApp.Frontend.BlazorApp.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private NavigationManager Navigation { get; set; }

    [Inject] private IAuthService Auth { get; set; }

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

    [Inject] private InvitationSignalRService InvitationHub { get; set; }

    [Inject] private IUsersService UsersService { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState> AuthStateTask { get; set; }

    private bool _drawerOpen = true;
    private string UserLetter = "?";
    private string? UserAvatarUrl = null;

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    protected override async Task OnInitializedAsync()
    {
        AuthStateProvider.AuthenticationStateChanged += OnAuthStateChanged;

        await UpdateUserStateAsync();
    }

    private async void OnAuthStateChanged(Task<AuthenticationState> task)
    {
        await InvokeAsync(async () =>
        {
            await UpdateUserStateAsync();
            StateHasChanged();
        });
    }

    private async Task UpdateUserStateAsync()
    {
        try
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var tagName = user.FindFirst(ClaimTypes.Name)?.Value;
                UserLetter = string.IsNullOrEmpty(tagName) ? "?" : tagName[0].ToString().ToUpper();

                var avatarClaim = user.FindFirst("AvatarUrl");
                if (!string.IsNullOrEmpty(avatarClaim?.Value))
                {
                    UserAvatarUrl = avatarClaim.Value;
                }

                StateHasChanged();

                var result = await UsersService.GetProfileAsync();

                if (result.IsSuccess)
                {
                    UserAvatarUrl = result.Value.AvatarUrl;

                    if (!string.IsNullOrEmpty(result.Value.DisplayName))
                    {
                        UserLetter = result.Value.DisplayName[0].ToString().ToUpper();
                    }
                }

                await InvitationHub.StartConnection();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user state: {ex.Message}");
        }
        finally
        {
            StateHasChanged();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (AuthStateTask != null)
        {
            var authState = await AuthStateTask;
            var user = authState.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                await LoadUserProfile(user);
            }
            else
            {
                UserLetter = "?";
                UserAvatarUrl = null;
            }
        }
    }

    private async Task LoadUserProfile(ClaimsPrincipal user)
    {
        try
        {
            var tagName = user.FindFirst(ClaimTypes.Name)?.Value;
            UserLetter = string.IsNullOrEmpty(tagName) ? "?" : tagName[0].ToString().ToUpper();

            var avatarClaim = user.FindFirst("AvatarUrl");
            if (!string.IsNullOrEmpty(avatarClaim?.Value))
            {
                UserAvatarUrl = avatarClaim.Value;
            }

            StateHasChanged();

            var result = await UsersService.GetProfileAsync();

            if (result.IsSuccess)
            {
                UserAvatarUrl = result.Value.AvatarUrl;

                if (!string.IsNullOrEmpty(result.Value.DisplayName))
                {
                    UserLetter = result.Value.DisplayName[0].ToString().ToUpper();
                }
            }

            await InvitationHub.StartConnection();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user state: {ex.Message}");
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task LogoutAsync()
    {
        await Auth.LogoutAsync();
        await InvitationHub.DisposeAsync();

        if (AuthStateProvider is CustomAuthStateProvider customProvider)
        {
            customProvider.NotifyUserLogout();
        }

        Navigation.NavigateTo("/login", true);
    }

    public void Dispose()
    {
        AuthStateProvider.AuthenticationStateChanged -= OnAuthStateChanged;
    }
}