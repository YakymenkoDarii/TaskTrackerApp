using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Features.Columns.Commands.CreateColumns;

public class CreateColumnCommandHandler : IRequestHandler<CreateColumnCommand, int>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public CreateColumnCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<int> Handle(CreateColumnCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var column = new Column
        {
            Title = request.Title,
            Description = request.Description,
            BoardId = request.BoardId,
        };

        var newId = await uow.ColumnRepository.AddAsync(column);

        await uow.SaveChangesAsync(cancellationToken);

        return newId;
    }
}