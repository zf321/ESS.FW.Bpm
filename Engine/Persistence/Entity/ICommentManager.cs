using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface ICommentManager
    {
        void Delete(CommentEntity dbEntity);
        void DeleteCommentsByTaskId(string taskId);
        CommentEntity FindCommentByTaskIdAndCommentId(string taskId, string commentId);
        IList<IComment> FindCommentsByProcessInstanceId(string processInstanceId);
        IList<IComment> FindCommentsByTaskId(string taskId);
        IList<IEvent> FindEventsByTaskId(string taskId);
        void Insert(CommentEntity dbEntity);
    }
}