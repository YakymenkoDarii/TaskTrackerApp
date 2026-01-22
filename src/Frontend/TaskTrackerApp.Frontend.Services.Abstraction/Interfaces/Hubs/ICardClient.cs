using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.Events.Comment;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Hubs;

public interface ICardClient
{
    Task CommentAdded(CommentAddedEvent e);

    Task CommentUpdated(CommentUpdatedEvent e);

    Task CommentDeleted(int commentId);

    Task LabelAdded(int cardId, int labelId);

    Task LabelRemoved(int cardId, int labelId);
}