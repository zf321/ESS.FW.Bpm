using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class ResolveTaskCmd : CompleteTaskCmd
    {
        private const long SerialVersionUid = 1L;

        public ResolveTaskCmd(string taskId, IDictionary<string, object> variables) : base(taskId, variables)
        {
        }

        protected internal override void CompleteTask(TaskEntity task)
        {
            task.Resolve();
            task.CreateHistoricTaskDetails(UserOperationLogEntryFields.OperationTypeResolve);
        }
    }
}