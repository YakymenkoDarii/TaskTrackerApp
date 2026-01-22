using Microsoft.AspNetCore.Components;
using MudBlazor;
using TaskTrackerApp.Frontend.Domain.DTOs.Labels;
using TaskTrackerApp.Frontend.Domain.Events.Labels;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Hubs;

namespace TaskTrackerApp.Frontend.BlazorApp.Components.Cards.Labels;

public partial class CardLabelsPanel : IDisposable
{
    [Inject] public ILabelService LabelService { get; set; }

    [Inject] public ISnackbar Snackbar { get; set; }

    [Inject] public IDialogService DialogService { get; set; }

    [Inject] public BoardSignalRService BoardHub { get; set; }

    [Inject] public CardSignalRService CardHub { get; set; }

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
        BoardHub.OnLabelCreated += HandleLabelCreated;
        BoardHub.OnLabelUpdated += HandleLabelUpdated;
        BoardHub.OnLabelDeleted += HandleLabelDeleted;

        CardHub.OnLabelAdded += HandleLabelAddedToCard;
        CardHub.OnLabelRemoved += HandleLabelRemovedFromCard;

        await LoadBoardLabels();
    }

    private void HandleLabelCreated(LabelCreatedEvent e)
    {
        InvokeAsync(() =>
        {
            if (e.BoardId == BoardId && !_allBoardLabels.Any(l => l.Id == e.LabelId))
            {
                _allBoardLabels.Add(new LabelDto { Id = e.LabelId, Name = e.Name, Color = e.Color });
                StateHasChanged();
            }
        });
    }

    private void HandleLabelUpdated(LabelUpdatedEvent e)
    {
        InvokeAsync(async () =>
        {
            if (e.BoardId == BoardId)
            {
                var label = _allBoardLabels.FirstOrDefault(l => l.Id == e.LabelId);
                if (label != null)
                {
                    label.Name = e.Name;
                    label.Color = e.Color;
                }
                var selected = SelectedLabels.FirstOrDefault(l => l.Id == e.LabelId);
                if (selected != null)
                {
                    selected.Name = e.Name;
                    selected.Color = e.Color;
                    await SelectedLabelsChanged.InvokeAsync(SelectedLabels);
                }

                StateHasChanged();
            }
        });
    }

    private void HandleLabelDeleted(LabelDeletedEvent e)
    {
        InvokeAsync(async () =>
        {
            if (e.BoardId == BoardId)
            {
                var label = _allBoardLabels.FirstOrDefault(l => l.Id == e.LabelId);
                if (label != null) _allBoardLabels.Remove(label);

                var selected = SelectedLabels.FirstOrDefault(l => l.Id == e.LabelId);
                if (selected != null)
                {
                    SelectedLabels.Remove(selected);
                    await SelectedLabelsChanged.InvokeAsync(SelectedLabels);
                }
                StateHasChanged();
            }
        });
    }

    private void HandleLabelAddedToCard(int cardId, int labelId)
    {
        InvokeAsync(async () =>
        {
            if (cardId == CardId && !SelectedLabels.Any(l => l.Id == labelId))
            {
                var labelDef = _allBoardLabels.FirstOrDefault(l => l.Id == labelId);
                if (labelDef != null)
                {
                    SelectedLabels.Add(labelDef);
                    await SelectedLabelsChanged.InvokeAsync(SelectedLabels);
                    StateHasChanged();
                }
            }
        });
    }

    private void HandleLabelRemovedFromCard(int cardId, int labelId)
    {
        InvokeAsync(async () =>
        {
            if (cardId == CardId)
            {
                var label = SelectedLabels.FirstOrDefault(l => l.Id == labelId);
                if (label != null)
                {
                    SelectedLabels.Remove(label);
                    await SelectedLabelsChanged.InvokeAsync(SelectedLabels);
                    StateHasChanged();
                }
            }
        });
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
                if (!_allBoardLabels.Any(l => l.Id == result.Value.Id))
                {
                    _allBoardLabels.Add(result.Value);
                }

                await ToggleLabel(result.Value);
            }
        }
        else
        {
            var updateDto = new LabelDto { Id = _editingLabel.Id, Name = _editingLabelName, Color = _editingLabelColor };
            var result = await LabelService.UpdateLabelAsync(updateDto);
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

    private string GetContrastColor(string hexColor) => "#FFFFFF";

    private string GetLabelStyle(LabelDto label)
    {
        return $"background-color: {label.Color}; color: {GetContrastColor(label.Color)}; font-size: 0.85rem;";
    }

    private string GetBorderStyle(string color)
    {
        return _editingLabelColor == color ? "2px solid black" : "1px solid #ddd";
    }

    public void Dispose()
    {
        BoardHub.OnLabelCreated -= HandleLabelCreated;
        BoardHub.OnLabelUpdated -= HandleLabelUpdated;
        BoardHub.OnLabelDeleted -= HandleLabelDeleted;

        CardHub.OnLabelAdded -= HandleLabelAddedToCard;
        CardHub.OnLabelRemoved -= HandleLabelRemovedFromCard;
    }
}