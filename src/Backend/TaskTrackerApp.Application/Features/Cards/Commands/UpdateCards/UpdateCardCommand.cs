using MediatR;
using TaskTrackerApp.Application.Features.Behaviors.Interfaces.Boards;
using TaskTrackerApp.Domain.DTOs.Card;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Cards.Commands.UpdateCards;

public class UpdateCardCommand : IRequest<Result<CardDto>>, IBoardRelatedCommand
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime? DueDate { get; set; }

    public int ColumnId { get; set; }

    public int BoardId { get; set; }

    public int? AssigneeId { get; set; }

    public int UpdatedById { get; set; }

    public bool IsCompleted { get; set; }

    public int Position { get; set; }

    public CardPriority Priority { get; set; }
}