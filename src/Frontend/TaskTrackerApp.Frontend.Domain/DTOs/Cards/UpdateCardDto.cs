using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Frontend.Domain.DTOs.Cards;

public class UpdateCardDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime? DueDate { get; set; }

    public int ColumnId { get; set; }

    public int BoardId { get; set; }

    public int? AssigneeId { get; set; }

    public int UpdatedById { get; set; }
}