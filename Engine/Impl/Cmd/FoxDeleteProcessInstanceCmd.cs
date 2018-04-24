using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    [Serializable]
    public class FoxDeleteProcessInstanceCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal string DeleteReason;
        protected internal string ProcessInstanceId;

        public FoxDeleteProcessInstanceCmd(string processInstanceId, string deleteReason)
        {
            this.ProcessInstanceId = processInstanceId;
            this.DeleteReason = deleteReason;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("processInstanceId", ProcessInstanceId);

            ExecutionEntity execution = commandContext.ExecutionManager.FindExecutionById(ProcessInstanceId);

            EnsureUtil.EnsureNotNull("No process instance found for id '" + ProcessInstanceId + "'", "execution",
              execution);

            commandContext.TaskManager.DeleteTasksByProcessInstanceId(ProcessInstanceId, DeleteReason, false, false);

            foreach (var currentExecution in this.CollectExecutionToDelete(execution))
            {
                currentExecution.DeleteCascade2(DeleteReason);
            }
            return null;
        }

        public virtual IList<PvmExecutionImpl> CollectExecutionToDelete(PvmExecutionImpl execution)
        {
            IList<PvmExecutionImpl> result = new List<PvmExecutionImpl>();
            foreach (var currentExecution in execution.Executions)
                ((List<PvmExecutionImpl>) result).AddRange(CollectExecutionToDelete((PvmExecutionImpl) currentExecution));
            if (execution.SubProcessInstance != null)
                ((List<PvmExecutionImpl>) result).AddRange(CollectExecutionToDelete((PvmExecutionImpl) execution.SubProcessInstance));
            result.Add(execution);
            return result;
        }
    }
}