using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class DeleteProcessInstanceCmd : AbstractDeleteProcessInstanceCmd, ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal string ProcessInstanceId;

        public DeleteProcessInstanceCmd(string processInstanceId, string deleteReason, bool skipCustomListeners)
            : this(processInstanceId, deleteReason, skipCustomListeners, false)
        {
        }

        public DeleteProcessInstanceCmd(string processInstanceId, string deleteReason, bool skipCustomListeners,
            bool externallyTerminated)
        {
            this.ProcessInstanceId = processInstanceId;
            this.DeleteReason = deleteReason;
            this.SkipCustomListeners = skipCustomListeners;
            this.ExternallyTerminated = externallyTerminated;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            DeleteProcessInstance(commandContext, ProcessInstanceId, DeleteReason, SkipCustomListeners,
                ExternallyTerminated);
            return null;
        }
    }
}