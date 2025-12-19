using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Models;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces;

namespace TaskTrackerApp.Frontend.WebApp.Components.Pages;

public partial class Login
{
    [Inject]
    public ISnackbar SnackBar { private get; set; } = default!;

    [Inject]
    public IAuthService AuthService { private get; set; } = default!;

    private readonly LoginModel model = new();

    private InputType PasswordInputType = InputType.Password;
    private string PasswordInputIcon = Icons.Material.Filled.Visibility;
    private bool isPasswordVisible = false;

    private EditContext _editContext = default!;
    private ValidationMessageStore _messageStore = default!;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(model);
        _messageStore = new ValidationMessageStore(_editContext);

        _editContext.OnFieldChanged += (s, e) =>
        {
            _messageStore.Clear(e.FieldIdentifier);
            _editContext.NotifyValidationStateChanged();
        };
    }

    private void TogglePasswordVisibility()
    {
        if (isPasswordVisible)
        {
            isPasswordVisible = false;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInputType = InputType.Password;
        }
        else
        {
            isPasswordVisible = true;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInputType = InputType.Text;
        }
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
        {
            request.Email = model.Login;
        }
        else
        {
            request.Tag = model.Login;
        }

        var result = await AuthService.LoginAsync(request);

        if (result.IsSuccess)
        {
            SnackBar.Add("Authorized", Severity.Success);
            return;
        }
        switch (result.Error.Code)
        {
            case var c when c == LoginError.InvalidPassword.Code:
                SnackBar.Add("Wrong password", Severity.Error);
                break;

            case var c when c == LoginError.UserNotFound.Code:
                SnackBar.Add("User not found", Severity.Error);
                break;

            case "Client.Network":
                SnackBar.Add("No internet connection", Severity.Warning);
                break;

            default:
                SnackBar.Add(result.Error.Message, Severity.Error);
                break;
        }
    }
}