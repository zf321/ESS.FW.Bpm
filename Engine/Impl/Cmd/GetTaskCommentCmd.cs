using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetTaskCommentCmd : ICommand<IComment>
    {
        private const long SerialVersionUid = 1L;
        protected internal string CommentId;
        protected internal string TaskId;

        public GetTaskCommentCmd(string taskId, string commentId)
        {
            this.TaskId = taskId;
            this.CommentId = commentId;
        }

        public virtual IComment Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("taskId", TaskId);
            EnsureUtil.EnsureNotNull("commentId", CommentId);
            return commandContext.CommentManager.FindCommentByTaskIdAndCommentId(TaskId, CommentId);
        }
    }
}