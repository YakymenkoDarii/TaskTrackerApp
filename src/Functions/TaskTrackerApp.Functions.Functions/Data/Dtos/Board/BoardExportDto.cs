using TaskTrackerApp.Functions.Functions.Data.Dtos.BoardMember;
using TaskTrackerApp.Functions.Functions.Data.Dtos.Column;
using TaskTrackerApp.Functions.Functions.Data.Dtos.Label;

namespace TaskTrackerApp.Functions.Functions.Data.Dtos.Board;

public class BoardExportDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsArchived { get; set; }

    public int CreatedById { get; set; }

    public List<BoardMemberDto> Members { get; set; } = new();

    public List<ColumnDto> Columns { get; set; } = new();

    public List<LabelDto> Labels { get; set; } = new();
}