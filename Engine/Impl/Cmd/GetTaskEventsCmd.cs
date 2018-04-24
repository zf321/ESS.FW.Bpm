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
    public class GetTaskEventsCmd : ICommand<IList<IEvent>>
    {
        private const long SerialVersionUid = 1L;
        protected internal string TaskId;

        public GetTaskEventsCmd(string taskId)
        {
            this.TaskId = taskId;
        }

        public virtual IList<IEvent> Execute(CommandContext commandContext)
        {
            return commandContext.CommentManager.FindEventsByTaskId(TaskId);
        }
    }
}