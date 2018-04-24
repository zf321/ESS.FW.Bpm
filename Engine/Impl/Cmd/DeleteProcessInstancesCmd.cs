using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class DeleteProcessInstancesCmd : AbstractDeleteProcessInstanceCmd, ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal IList<string> ProcessInstanceIds;

        public DeleteProcessInstancesCmd(IList<string> processInstanceIds, string deleteReason, bool skipCustomListeners,
            bool externallyTerminated)
        {
            this.ProcessInstanceIds = processInstanceIds;
            this.DeleteReason = deleteReason;
            this.SkipCustomListeners = skipCustomListeners;
            this.ExternallyTerminated = externallyTerminated;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            foreach (var processInstanceId in ProcessInstanceIds)
                DeleteProcessInstance(commandContext, processInstanceId, DeleteReason, SkipCustomListeners,
                    ExternallyTerminated);
            return null;
        }
    }
}