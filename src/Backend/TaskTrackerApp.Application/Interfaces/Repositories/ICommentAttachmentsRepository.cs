using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Repositories;

public interface ICommentAttachmentsRepository : IRepository<CommentAttachment, int>
{
}