using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetSubTasksCmd : ICommand<IList<ITask>>
    {
        private const long SerialVersionUid = 1L;
        protected internal string ParentTaskId;

        public GetSubTasksCmd(string parentTaskId)
        {
            this.ParentTaskId = parentTaskId;
        }

        public virtual IList<ITask> Execute(CommandContext commandContext)
        {
            return commandContext.TaskManager.FindTasksByParentTaskId(ParentTaskId)
                .ToList().Cast<ITask>().ToList();
        }
    }
}