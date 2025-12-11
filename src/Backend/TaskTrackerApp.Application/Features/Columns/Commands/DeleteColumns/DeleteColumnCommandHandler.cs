using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Features.Boards.Commands.DeleteBoards;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Application.Features.Columns.Commands.DeleteColumns;
internal class DeleteColumnCommandHandler : IRequestHandler<DeleteColumnCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public DeleteColumnCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }
    public async Task Handle(DeleteColumnCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        await uow.ColumnRepository.DeleteAsync(request.Id);

        await uow.SaveChangesAsync(cancellationToken);
    }
}
