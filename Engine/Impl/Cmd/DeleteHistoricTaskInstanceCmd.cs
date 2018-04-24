using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class DeleteHistoricTaskInstanceCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal string TaskId;

        public DeleteHistoricTaskInstanceCmd(string taskId)
        {
            this.TaskId = taskId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("taskId", TaskId);

            HistoricTaskInstanceEventEntity task =
                commandContext.HistoricTaskInstanceManager.FindHistoricTaskInstanceById(TaskId);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckDeleteHistoricTaskInstance(task);
            }

            commandContext.HistoricTaskInstanceManager.DeleteHistoricTaskInstanceById(TaskId);
            return null;
        }
    }
}