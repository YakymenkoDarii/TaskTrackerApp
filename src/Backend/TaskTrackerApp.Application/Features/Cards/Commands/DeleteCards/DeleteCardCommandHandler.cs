using MediatR;
using TaskTrackerApp.Application.Features.Cards.Commands.DeleteCard;
using TaskTrackerApp.Application.Interfaces.BlobStorage;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Constants; // Assuming this has your container name
using TaskTrackerApp.Domain.Events.Card;

namespace TaskTrackerApp.Application.Features.Cards.Commands.DeleteCards;

internal class DeleteCardCommandHandler : IRequestHandler<DeleteCardCommand>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBoardNotifier _boardNotifier;
    private readonly IBlobStorageService _blobService;

    public DeleteCardCommandHandler(
        IUnitOfWorkFactory uowFactory,
        IBoardNotifier boardNotifier,
        IBlobStorageService blobService)
    {
        _uowFactory = uowFactory;
        _boardNotifier = boardNotifier;
        _blobService = blobService;
    }

    public async Task Handle(DeleteCardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var card = await uow.CardRepository.GetById(request.Id);
        if (card == null) return;

        int boardId = card.BoardId;

        await uow.CardRepository.DeleteAsync(request.Id);
        await uow.SaveChangesAsync(cancellationToken);

        var folderPrefix = $"card-{request.Id}/";

        await _blobService.DeleteFolderAsync(BlobContainerNames.CommentAttachments, folderPrefix);

        var evt = new CardDeletedEvent(request.Id, boardId);
        _ = _boardNotifier.NotifyCardDeletedAsync(evt);
    }
}