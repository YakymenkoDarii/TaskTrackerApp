using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth;
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

        if (!result.IsSuccess)
        {
            SnackBar.Add("Wrong login or password");
        }

        _editContext.NotifyValidationStateChanged();
    }
}