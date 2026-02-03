using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Functions.Functions.Data.Dtos.Card;

namespace TaskTrackerApp.Functions.Functions.Data.Dtos.Column;

public class ColumnDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int Position { get; set; }

    public List<CardDto> Cards { get; set; } = new();
}