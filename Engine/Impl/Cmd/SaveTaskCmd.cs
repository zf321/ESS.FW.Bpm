using System;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class SaveTaskCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal TaskEntity Task;

        public SaveTaskCmd(ITask task)
        {
            this.Task = (TaskEntity) task;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("Task", Task);

            string operation=string.Empty;

            if (Task.Revision == 0)
                try
                {
                    CheckCreateTask(Task, commandContext);
                    Task.SaveTask(null);
                    Task.Update();
                    commandContext.HistoricTaskInstanceManager.CreateHistoricTask(Task);
                    operation = UserOperationLogEntryFields.OperationTypeCreate;
                    Task.ExecuteMetrics(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart);
                }
                catch (NullValueException e)
                {
                    throw new NotValidException(e.Message, e);
                }
            else
            {
                CheckTaskAssign(Task, commandContext);
                Task.Update();
                operation = UserOperationLogEntryFields.OperationTypeUpdate;
            }
                

            Task.FireAuthorizationProvider();
            Task.FireEvent();
            Task.CreateHistoricTaskDetails(operation);

            return null;
        }

        protected internal virtual void CheckTaskAssign(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckTaskAssign(task);
        }

        protected internal virtual void CheckCreateTask(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckCreateTask(task);
        }
    }
}