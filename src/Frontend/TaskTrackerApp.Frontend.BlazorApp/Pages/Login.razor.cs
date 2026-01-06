using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth.Requests;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Models;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Auth;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages;

public partial class Login
{
    [Inject] public ISnackbar SnackBar { get; set; } = default!;
    [Inject] public IAuthService AuthService { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    [Parameter]
    public string? returnUrl { get; set; }

    private readonly LoginModel model = new();

    private InputType PasswordInputType = InputType.Password;
    private string PasswordInputIcon = Icons.Material.Filled.Visibility;
    private bool isPasswordVisible;

    private EditContext _editContext = default!;
    private ValidationMessageStore _messageStore = default!;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(model);
        _messageStore = new ValidationMessageStore(_editContext);

        _editContext.OnFieldChanged += (_, e) =>
        {
            _messageStore.Clear(e.FieldIdentifier);
            _editContext.NotifyValidationStateChanged();
        };
    }

    private void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;
        PasswordInputIcon = isPasswordVisible
            ? Icons.Material.Filled.VisibilityOff
            : Icons.Material.Filled.Visibility;

        PasswordInputType = isPasswordVisible
            ? InputType.Text
            : InputType.Password;
    }

    private async Task OnValidSubmit()
    {
        _messageStore.Clear();
        _editContext.NotifyValidationStateChanged();

        var request = new LoginRequest
        {
            Password = model.Password
        };

        if (model.Login.Contains('@'))
            request.Email = model.Login;
        else
            request.Tag = model.Login;

        var result = await AuthService.LoginAsync(request);

        if (!result.IsSuccess)
        {
            HandleError(result.Error);
            return;
        }

        ((CustomAuthStateProvider)AuthStateProvider)
        .NotifyUserAuthentication(result.Value.AccessToken);

        var target = returnUrl ?? "/";
        Navigation.NavigateTo(target);
    }

    private void HandleError(Error error)
    {
        switch (error.Code)
        {
            case var c when c == LoginError.InvalidPassword.Code:
                SnackBar.Add("Wrong password", Severity.Error);
                break;

            case var c when c == LoginError.UserNotFound.Code:
                SnackBar.Add("User not found", Severity.Error);
                break;

            case ClientErrors.NetworkErrorCode:
                SnackBar.Add("No internet connection", Severity.Warning);
                break;

            default:
                SnackBar.Add(error.Message, Severity.Error);
                break;
        }
    }
}