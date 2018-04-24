using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetTaskCommentsCmd : ICommand<IList<IComment>>
    {
        private const long SerialVersionUid = 1L;
        protected internal string TaskId;

        public GetTaskCommentsCmd(string taskId)
        {
            this.TaskId = taskId;
        }

        public virtual IList<IComment> Execute(CommandContext commandContext)
        {
            return commandContext.CommentManager.FindCommentsByTaskId(TaskId);
        }
    }
}