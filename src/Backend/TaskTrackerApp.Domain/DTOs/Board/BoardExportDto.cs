using TaskTrackerApp.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Domain.DTOs.Column;

namespace TaskTrackerApp.Domain.DTOs.Board;

public class BoardExportDto : BoardDto
{
    public List<BoardMemberDto> Members { get; set; } = new();

    public List<ColumnExportDto> Columns { get; set; } = new();
}