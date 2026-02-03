using TaskTrackerApp.Domain.DTOs.Card;

namespace TaskTrackerApp.Domain.DTOs.Column;

public class ColumnExportDto : ColumnDto
{
    public List<CardExportDto> Cards { get; set; } = new();
}