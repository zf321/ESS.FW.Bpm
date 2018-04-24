using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class DeleteHistoricProcessInstanceCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal string ProcessInstanceId;

        public DeleteHistoricProcessInstanceCmd(string processInstanceId)
        {
            this.ProcessInstanceId = processInstanceId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("processInstanceId", ProcessInstanceId);
            // Check if process instance is still running
            IHistoricProcessInstance instance =
                commandContext.HistoricProcessInstanceManager.FindHistoricProcessInstance(ProcessInstanceId);

            EnsureUtil.EnsureNotNull("No historic process instance found with id: " + ProcessInstanceId, "instance",
                instance);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckDeleteHistoricProcessInstance(instance);
            }

            EnsureUtil.EnsureNotNull(
                "Process instance is still running, cannot delete historic process instance: " + ProcessInstanceId,
                "instance.getEndTime()", instance.EndTime);

            commandContext.HistoricProcessInstanceManager.DeleteHistoricProcessInstanceById(ProcessInstanceId);

            return null;
        }
    }
}