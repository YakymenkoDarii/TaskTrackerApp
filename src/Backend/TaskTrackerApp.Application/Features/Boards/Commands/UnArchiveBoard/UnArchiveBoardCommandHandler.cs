using MediatR;

using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.BackupDtos;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.UnArchiveBoard;

internal class UnArchiveBoardCommandHandler : IRequestHandler<UnArchiveBoardCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBoardBackupService _backupService;

    public UnArchiveBoardCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService,
        IBoardBackupService backupService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
        _backupService = backupService;
    }

    public async Task<Result> Handle(UnArchiveBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var archivedBoard = await uow.ArchivedBoardsRepository.GetById(request.BoardId);
        var checkBoard = await uow.BoardRepository.GetById(archivedBoard.OriginalBoardId);

        if (checkBoard != null)
        {
            checkBoard.IsQueuedForArchival = false;
            checkBoard.IsArchived = false;

            await uow.ArchivedBoardsRepository.DeleteAsync(request.BoardId);
            await uow.SaveChangesAsync();
            return Result.Success();
        }

        var userId = _currentUserService.UserId;
        if (userId == null) return UserErrors.NotFound;

        var memberRecord = await uow.ArchivedBoardMembersRepository
            .GetMemberAsync(request.BoardId, userId.Value, cancellationToken);

        if (memberRecord == null || memberRecord.Role != BoardRole.Admin)
        {
            return Result.Failure(new Error("Not aurhorized", "You do not have permision to restore this board"));
        }

        var backupDto = await _backupService.GetBackupAsync(archivedBoard.OriginalBoardId, cancellationToken);
        if (backupDto == null)
        {
            return Result.Failure(new Error("Backup.Failure", "Backup data not found. The board cannot be restored."));
        }

        var newBoard = new Board
        {
            Title = backupDto.Title,
            Description = backupDto.Description,
            CreatedById = backupDto.CreatedById,
            UpdatedById = _currentUserService.UserId ?? backupDto.CreatedById,
            IsArchived = false,
            IsQueuedForArchival = false,
            CreatedAt = DateTime.UtcNow
        };

        var archivedMembers = await uow.ArchivedBoardMembersRepository
            .GetMembersByBoardIdAsync(request.BoardId, cancellationToken);

        foreach (var archMember in archivedMembers)
        {
            newBoard.Members.Add(new BoardMember
            {
                UserId = archMember.UserId,
                Role = archMember.Role
            });
        }

        await uow.BoardRepository.AddAsync(newBoard);

        await MapAndRestoreChildren(newBoard, backupDto, uow, cancellationToken);

        var archivedBoardEntry = await uow.ArchivedBoardsRepository.GetById(request.BoardId);
        if (archivedBoardEntry != null)
        {
            await uow.ArchivedBoardsRepository.DeleteAsync(archivedBoardEntry.Id);
        }

        await uow.SaveChangesAsync(cancellationToken);

        await _backupService.DeleteBackupAsync(archivedBoard.OriginalBoardId, cancellationToken);

        return Result.Success();
    }

    private async Task MapAndRestoreChildren(Board newBoard, BoardBackupDto dto, IUnitOfWork uow, CancellationToken ct)
    {
        var labelMap = new Dictionary<int, Label>();

        int currentUserId = _currentUserService.UserId ?? newBoard.CreatedById;
        var now = DateTime.UtcNow;

        if (dto.Labels.Any())
        {
            foreach (var labelDto in dto.Labels)
            {
                var newLabel = new Label
                {
                    Name = labelDto.Name,
                    Color = labelDto.Color,
                    Board = newBoard,
                    CreatedById = currentUserId,
                    UpdatedById = currentUserId,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                await uow.LabelsRepository.AddAsync(newLabel);

                await uow.SaveChangesAsync(ct);

                labelMap[labelDto.Id] = newLabel;
            }
        }

        foreach (var colDto in dto.Columns)
        {
            var newCol = new Column
            {
                Title = colDto.Title,
                Position = colDto.Position,
                Board = newBoard,
            };

            await uow.ColumnRepository.AddAsync(newCol);

            foreach (var cardDto in colDto.Cards)
            {
                var newCard = new Card
                {
                    Title = cardDto.Title,
                    Description = cardDto.Description,
                    Position = cardDto.Position,
                    IsCompleted = cardDto.IsCompleted,
                    Priority = (CardPriority)cardDto.Priority,
                    DueDate = cardDto.DueDate,
                    AssigneeId = cardDto.AssigneeId,
                    Column = newCol,
                    Board = newBoard,
                    CreatedById = currentUserId,
                    UpdatedById = currentUserId,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                foreach (var commDto in cardDto.Comments)
                {
                    var newComment = new CardComment
                    {
                        Text = commDto.Text,
                        CreatedById = commDto.CreatedById,
                        CreatedAt = commDto.CreatedAt,
                        UpdatedById = commDto.CreatedById,
                        UpdatedAt = commDto.CreatedAt
                    };

                    foreach (var attDto in commDto.Attachments)
                    {
                        newComment.Attachments.Add(new CommentAttachment
                        {
                            FileName = attDto.FileName,
                            StoredFileName = attDto.StoredFileName,
                            ContentType = attDto.ContentType,
                            Url = attDto.Url
                        });
                    }
                    newCard.Comments.Add(newComment);
                }

                foreach (var oldLabelId in cardDto.LabelIds)
                {
                    if (labelMap.TryGetValue(oldLabelId, out var trackedLabel))
                    {
                        newCard.Labels.Add(trackedLabel);
                    }
                }

                await uow.CardRepository.AddAsync(newCard);
            }
        }
    }
}