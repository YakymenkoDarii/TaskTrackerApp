using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth;
using TaskTrackerApp.Frontend.Domain.Models;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces;

namespace TaskTrackerApp.Frontend.WebApp.Components.Pages;

public partial class Signup
{
    [Inject]
    public IAuthService AuthService { private get; set; } = default!;

    [Inject]
    public ISnackbar Snackbar { private get; set; } = default!;

    private readonly SignupModel model = new();

    private InputType PasswordInputType = InputType.Password;
    private string PasswordInputIcon = Icons.Material.Filled.Visibility;
    private bool isPasswordVisible = false;

    private InputType RepeatPasswordInputType = InputType.Password;
    private string RepeatPasswordInputIcon = Icons.Material.Filled.Visibility;
    private bool isRepeatPasswordVisible = false;

    private EditContext _editContext = default!;
    private ValidationMessageStore _messageStore = default!;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(model);
        _messageStore = new ValidationMessageStore(_editContext);

        _editContext.OnFieldChanged += (s, e) =>
        {
            _messageStore.Clear(e.FieldIdentifier);

            if (e.FieldIdentifier.FieldName == nameof(SignupModel.Password))
            {
                var repeatField = _editContext.Field(nameof(SignupModel.ConfirmPassword));
                _editContext.NotifyFieldChanged(repeatField);
            }

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

    private void ToggleRepeatPasswordVisibility()
    {
        if (isRepeatPasswordVisible)
        {
            isRepeatPasswordVisible = false;
            RepeatPasswordInputIcon = Icons.Material.Filled.Visibility;
            RepeatPasswordInputType = InputType.Password;
        }
        else
        {
            isRepeatPasswordVisible = true;
            RepeatPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            RepeatPasswordInputType = InputType.Text;
        }
    }

    private async Task OnValidSubmit()
    {
        _messageStore.Clear();
        _editContext.NotifyValidationStateChanged();

        var request = new SignupRequest
        {
            Email = model.Email,
            DisplayName = model.DisplayName,
            Password = model.Password,
            Tag = model.Tag
        };

        var result = await AuthService.SignupAsync(request);

        if (result.IsSuccess)
        {
            Snackbar.Add("Signed up", Severity.Success);
            return;
        }

        if (!result.IsSuccess)
        {
            Snackbar.Add("Something went wrong. Please try again later.", Severity.Error);
        }

        _editContext.NotifyValidationStateChanged();
    }
}