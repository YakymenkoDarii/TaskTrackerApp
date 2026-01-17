using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Labels;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.BlazorApp.Components.Cards.Labels;

public partial class CardLabelsPanel
{
    [Inject] public ILabelService LabelService { get; set; }

    [Inject] public ISnackbar Snackbar { get; set; }

    [Parameter] public int BoardId { get; set; }

    [Parameter] public int CardId { get; set; }

    [Parameter] public List<LabelDto> SelectedLabels { get; set; } = new();

    [Parameter] public EventCallback<List<LabelDto>> SelectedLabelsChanged { get; set; }

    [Parameter] public bool IsReadOnly { get; set; }

    private bool _isPopoverOpen;
    private bool _isCreatingOrEditing;
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
        var result = await LabelService.GetLabelsByBoardId(BoardId);
        if (result.IsSuccess)
        {
            _allBoardLabels = result.Value.ToList();
        }
    }

    private void TogglePopover() => _isPopoverOpen = !_isPopoverOpen;

    private async Task ToggleLabel(LabelDto label)
    {
        var existingLabel = SelectedLabels.FirstOrDefault(l => l.Id == label.Id);

        if (existingLabel != null)
        {
            SelectedLabels.Remove(existingLabel);

            await LabelService.RemoveLabelFromCard(CardId, label.Id);
        }
        else
        {
            SelectedLabels.Add(label);

            await LabelService.AddLabelToCard(CardId, label.Id);
        }

        await SelectedLabelsChanged.InvokeAsync(SelectedLabels);
    }

    private void StartCreate()
    {
        _editingLabel = null;
        _editingLabelName = "";
        _editingLabelColor = _presetColors[0];
        _isCreatingOrEditing = true;
    }

    private void StartEdit(LabelDto label)
    {
        _editingLabel = label;
        _editingLabelName = label.Name;
        _editingLabelColor = label.Color;
        _isCreatingOrEditing = true;
    }

    private async Task SaveLabel()
    {
        if (string.IsNullOrWhiteSpace(_editingLabelName)) return;

        if (_editingLabel == null)
        {
            var createDto = new CreateLabelDto { Name = _editingLabelName, Color = _editingLabelColor, BoardId = BoardId };
            var result = await LabelService.CreateLabel(createDto);
            if (result.IsSuccess)
            {
                _allBoardLabels.Add(result.Value);
                await ToggleLabel(result.Value);
            }
        }
        else
        {
            var updateDto = new LabelDto { Id = _editingLabel.Id, Name = _editingLabelName, Color = _editingLabelColor };
            var result = await LabelService.UpdateLabel(updateDto);
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

        _isCreatingOrEditing = false;
    }

    private string GetContrastColor(string hexColor)
    {
        return "#FFFFFF";
    }
}