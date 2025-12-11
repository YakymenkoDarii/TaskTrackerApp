using MediatR;

namespace TaskTrackerApp.Application.Features.Cards.Commands.CreateCard;

public class CreateCardCommand : IRequest<int>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int ColumnId { get; set; }
    public int BoardId { get; set; }
    public int? AssigneeId { get; set; }
}