using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Columns.Commands.UpdateColumns;

internal class UpdateColumnCommandHandler : IRequestHandler<UpdateColumnCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public UpdateColumnCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task Handle(UpdateColumnCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var column = await uow.ColumnRepository.GetAsync(request.Id);

        column.Title = request.Title;
        column.Description = request.Description;
        column.BoardId = request.BoardId;
        column.UpdatedAt = DateTime.UtcNow;

        await uow.ColumnRepository.UpdateAsync(column);

        await uow.SaveChangesAsync(cancellationToken);
    }
}