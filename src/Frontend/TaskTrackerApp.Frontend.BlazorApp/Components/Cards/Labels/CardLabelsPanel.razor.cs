using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Labels;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Components.Cards.Labels;

public partial class CardLabelsPanel
{
    [Inject] public ILabelService LabelService { get; set; }

    [Inject] public ISnackbar Snackbar { get; set; }

    [Inject] public IDialogService DialogService { get; set; }

    [Parameter] public int BoardId { get; set; }

    [Parameter] public int CardId { get; set; }

    [Parameter] public List<LabelDto> SelectedLabels { get; set; } = new();

    [Parameter] public EventCallback<List<LabelDto>> SelectedLabelsChanged { get; set; }

    [Parameter] public bool IsReadOnly { get; set; }

    private bool _isCreatingOrEditing;
    private bool _isCreatingNew;
    private List<LabelDto> _allBoardLabels = new();

    private string _searchTerm = "";

    private IEnumerable<LabelDto> FilteredBoardLabels =>
        string.IsNullOrWhiteSpace(_searchTerm)
        ? _allBoardLabels
        : _allBoardLabels.Where(l => l.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase));

    private LabelDto? _editingLabel;
    private string _editingLabelName = "";
    private string _editingLabelColor = "#2196F3";

    private readonly List<string> _presetColors = new()
    {
        "#61BD4F", "#F2D600", "#FF9F1A", "#EB5A46", "#C377E0", "#0079BF", "#00C2E0", "#51E898", "#FF78CB", "#344563"
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadBoardLabels();
    }

    private async Task LoadBoardLabels()
    {
        var result = await LabelService.GetLabelsByBoardIdAsync(BoardId);
        if (result.IsSuccess)
        {
            _allBoardLabels = result.Value.ToList();
        }
    }

    private async Task ToggleLabel(LabelDto label)
    {
        var existingLabel = SelectedLabels.FirstOrDefault(l => l.Id == label.Id);

        if (existingLabel != null)
        {
            SelectedLabels.Remove(existingLabel);

            await LabelService.RemoveLabelFromCardAsync(CardId, label.Id);
        }
        else
        {
            SelectedLabels.Add(label);

            await LabelService.AddLabelToCardAsync(CardId, label.Id);
        }

        await SelectedLabelsChanged.InvokeAsync(SelectedLabels);
    }

    private void StartCreate()
    {
        _editingLabel = null;
        _editingLabelName = "";
        _editingLabelColor = _presetColors[0];
        _isCreatingOrEditing = true;
        _isCreatingNew = false;
        _searchTerm = "";
    }

    private void StartEdit(LabelDto label)
    {
        _editingLabel = label;
        _editingLabelName = label.Name;
        _editingLabelColor = label.Color;
        _isCreatingOrEditing = true;
        _isCreatingNew = false;
    }

    private async Task SaveLabel()
    {
        if (string.IsNullOrWhiteSpace(_editingLabelName)) return;

        if (_editingLabel == null)
        {
            var createDto = new CreateLabelDto { Name = _editingLabelName, Color = _editingLabelColor, BoardId = BoardId };
            var result = await LabelService.CreateLabelAsync(createDto);
            if (result.IsSuccess)
            {
                _allBoardLabels.Add(result.Value);

                await ToggleLabel(result.Value);
            }
        }
        else
        {
            var updateDto = new LabelDto { Id = _editingLabel.Id, Name = _editingLabelName, Color = _editingLabelColor };
            var result = await LabelService.UpdateLabelAsync(updateDto);
            if (result.IsSuccess)
            {
                var index = _allBoardLabels.FindIndex(l => l.Id == _editingLabel.Id);
                if (index != -1) _allBoardLabels[index] = result.Value;

                var selectedIndex = SelectedLabels.FindIndex(l => l.Id == _editingLabel.Id);
                if (selectedIndex != -1)
                {
                    SelectedLabels[selectedIndex] = result.Value;
                    await SelectedLabelsChanged.InvokeAsync(SelectedLabels);
                }
            }
        }

        _editingLabel = null;
        _isCreatingNew = false;
    }

    private async Task DeleteLabel()
    {
        if (_editingLabel == null) return;

        bool? confirm = await DialogService.ShowMessageBox(
            "Delete Label",
            "Are you sure? This will remove this label from ALL cards on this board.",
            yesText: "Delete", cancelText: "Cancel");

        if (confirm == true)
        {
            var result = await LabelService.DeleteLabelAsync(_editingLabel.Id);

            if (result.IsSuccess)
            {
                var labelToRemove = _allBoardLabels.FirstOrDefault(l => l.Id == _editingLabel.Id);
                if (labelToRemove != null) _allBoardLabels.Remove(labelToRemove);

                var selectedToRemove = SelectedLabels.FirstOrDefault(l => l.Id == _editingLabel.Id);
                if (selectedToRemove != null)
                {
                    SelectedLabels.Remove(selectedToRemove);
                    await SelectedLabelsChanged.InvokeAsync(SelectedLabels);
                }

                _editingLabel = null;
                _isCreatingNew = false;
                Snackbar.Add("Label deleted", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error.Message, Severity.Error);
            }
        }
    }

    private string GetContrastColor(string hexColor)
    {
        return "#FFFFFF";
    }

    private string GetLabelStyle(LabelDto label)
    {
        return $"background-color: {label.Color}; color: {GetContrastColor(label.Color)}; font-size: 0.85rem;";
    }

    private string GetBorderStyle(string color)
    {
        return _editingLabelColor == color ? "2px solid black" : "1px solid #ddd";
    }
}