using MediatR;

namespace TaskTrackerApp.Application.Features.Cards.Commands.UpdateCards;

public class UpdateCardCommand : IRequest
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime? DueDate { get; set; }

    public int ColumnId { get; set; }

    public int BoardId { get; set; }

    public int? AssigneeId { get; set; }

    public int UpdatedBy { get; set; }
}