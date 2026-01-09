using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace TaskTrackerApp.Frontend.BlazorApp.Components;

public partial class EditableLabel
{
    [Parameter] public string Value { get; set; }

    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    [Parameter] public EventCallback<string> OnCommit { get; set; }

    [Parameter] public Typo Typo { get; set; } = Typo.body1;

    [Parameter] public string Class { get; set; }

    [Parameter] public string Style { get; set; }

    [Parameter] public string Placeholder { get; set; } = "Click to edit";

    [Parameter] public bool Multiline { get; set; } = false;

    [Parameter] public bool FullWidth { get; set; } = false;

    private bool _isEditing;
    private MudTextField<string>? _textField;

    private async Task EnableEdit()
    {
        if (_isEditing)
        {
            return;
        }

        _isEditing = true;

        await Task.Delay(50);

        if (_textField != null)
        {
            await _textField.FocusAsync();
        }
    }

    private async Task OnValueChanged(string newValue)
    {
        Value = newValue;
        await ValueChanged.InvokeAsync(newValue);
    }

    private async Task FinishEdit()
    {
        if (!_isEditing)
        {
            return;
        }

        _isEditing = false;
        await OnCommit.InvokeAsync(Value);
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (!Multiline && e.Key == "Enter")
        {
            await FinishEdit();
        }

        if (Multiline && e.CtrlKey && e.Key == "Enter")
        {
            await FinishEdit();
        }
    }
}