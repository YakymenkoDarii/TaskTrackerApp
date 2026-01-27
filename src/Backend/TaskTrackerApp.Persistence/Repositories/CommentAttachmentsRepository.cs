using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class CommentAttachmentsRepository : Repository<CommentAttachment, int>, ICommentAttachmentsRepository
{
    public CommentAttachmentsRepository(TaskTrackerDbContext context) : base(context)
    {
    }
}