using MediatR;

namespace TaskTrackerApp.Application.Features.Boards.Commands.DeleteBoards;

public class DeleteBoardCommand : IRequest
{
    public int Id { get; set; }
}