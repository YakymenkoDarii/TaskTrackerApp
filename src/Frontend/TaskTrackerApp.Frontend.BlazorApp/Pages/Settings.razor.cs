using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Users;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Errors.User;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Pages;

public partial class Settings
{
    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] private IUsersService UsersService { get; set; }

    private UpdateUserDto _profileModel = new();
    private ChangePasswordRequest _passwordModel = new();

    private EditContext _profileEditContext;
    private ValidationMessageStore _profileMessageStore;

    private EditContext _passwordEditContext;
    private ValidationMessageStore _passwordMessageStore;

    private string? _avatarUrl;
    private string _initials = "?";
    private bool _isSavingProfile = false;
    private bool _isSavingPassword = false;

    private const string TagLabel = "User Tag (@handle)";

    protected override async Task OnInitializedAsync()
    {
        _profileEditContext = new EditContext(_profileModel);
        _profileMessageStore = new ValidationMessageStore(_profileEditContext);
        _profileEditContext.OnFieldChanged += (s, e) =>
        {
            _profileMessageStore.Clear(e.FieldIdentifier);
            _profileEditContext.NotifyValidationStateChanged();
        };

        _passwordEditContext = new EditContext(_passwordModel);
        _passwordMessageStore = new ValidationMessageStore(_passwordEditContext);
        _passwordEditContext.OnFieldChanged += (s, e) =>
        {
            _passwordMessageStore.Clear(e.FieldIdentifier);
            _passwordEditContext.NotifyValidationStateChanged();
        };

        await LoadProfileAsync();
    }

    private async Task LoadProfileAsync()
    {
        var result = await UsersService.GetProfileAsync();

        if (result.IsSuccess)
        {
            var profile = result.Value;

            _profileModel.DisplayName = profile.DisplayName;
            _profileModel.Tag = profile.Tag;
            _avatarUrl = profile.AvatarUrl;

            if (!string.IsNullOrEmpty(_profileModel.DisplayName))
                _initials = _profileModel.DisplayName[0].ToString().ToUpper();
        }
        else
        {
            Snackbar.Add("Failed to load profile settings.", Severity.Error);
        }
    }

    private async Task UploadAvatar(IBrowserFile file)
    {
        if (file.Size > 5120000)
        {
            Snackbar.Add("File size must be less than 5MB", Severity.Warning);
            return;
        }

        var result = await UsersService.UpdateAvatarAsync(file);

        if (result.IsSuccess)
        {
            _avatarUrl = result.Value.ToString();
            Snackbar.Add("Avatar updated!", Severity.Success);
        }
        else
        {
            Snackbar.Add(result.Error.Message, Severity.Error);
        }
    }

    private async Task UpdateProfileAsync()
    {
        _profileMessageStore.Clear();
        _profileEditContext.NotifyValidationStateChanged();

        _isSavingProfile = true;

        var result = await UsersService.UpdateAsync(_profileModel);

        if (result.IsSuccess)
        {
            Snackbar.Add("Profile updated successfully.", Severity.Success);
        }
        else
        {
            HandleProfileError(result.Error);
        }

        _isSavingProfile = false;
    }

    private async Task ChangePasswordAsync()
    {
        _passwordMessageStore.Clear();
        _passwordEditContext.NotifyValidationStateChanged();

        if (!_passwordEditContext.Validate()) return;

        _isSavingPassword = true;

        var result = await UsersService.ChangePassword(_passwordModel);

        if (result.IsSuccess)
        {
            Snackbar.Add("Password changed successfully.", Severity.Success);

            _passwordModel = new ChangePasswordRequest();
            _passwordEditContext = new EditContext(_passwordModel);
            _passwordMessageStore = new ValidationMessageStore(_passwordEditContext);
        }
        else
        {
            HandlePasswordError(result.Error);
        }

        _isSavingPassword = false;
    }

    private void HandleProfileError(Error error)
    {
        if (error == null)
        {
            Snackbar.Add("An unknown error occurred.", Severity.Error);
            return;
        }

        if (error.Code == UserErrors.TagInUse.Code)
        {
            if (_profileEditContext != null)
            {
                _profileMessageStore.Add(_profileEditContext.Field(nameof(_profileModel.Tag)), error.Message);
                _profileEditContext.NotifyValidationStateChanged();
            }
        }
        else
        {
            Snackbar.Add(error.Message, Severity.Error);
        }
    }

    private void HandlePasswordError(Error error)
    {
        if (error.Code == UserErrors.InvalidPassword.Code)
        {
            _passwordMessageStore.Add(_passwordEditContext.Field(nameof(_passwordModel.OldPassword)), error.Message);
            _passwordEditContext.NotifyValidationStateChanged();
        }
        else if (error.Code == UserErrors.PasswordMismatch.Code)
        {
            _passwordMessageStore.Add(_passwordEditContext.Field(nameof(_passwordModel.ConfirmPassword)), error.Message);
            _passwordEditContext.NotifyValidationStateChanged();
        }
        else
        {
            Snackbar.Add(error.Message, Severity.Error);
        }
    }

    private string? PasswordMatchValidation(string arg)
    {
        if (_passwordModel.NewPassword != arg)
            return "Passwords do not match";
        return null;
    }
}