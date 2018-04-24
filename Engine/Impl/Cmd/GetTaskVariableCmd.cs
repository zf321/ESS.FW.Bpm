using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetTaskVariableCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal bool IsLocal;
        protected internal string TaskId;
        protected internal string VariableName;

        public GetTaskVariableCmd(string taskId, string variableName, bool isLocal)
        {
            this.TaskId = taskId;
            this.VariableName = variableName;
            this.IsLocal = isLocal;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("taskId", TaskId);
            EnsureUtil.EnsureNotNull("variableName", VariableName);

            TaskEntity task = context.Impl.Context.CommandContext.TaskManager.FindTaskById(TaskId);

            EnsureUtil.EnsureNotNull("ITask " + TaskId + " doesn't exist", "ITask", task);

            CheckGetTaskVariable(task, commandContext);

            object value;

            if (IsLocal)
            {
                value = task.GetVariableLocal(VariableName);
            }
            else
            {
                value = task.GetVariable(VariableName);
            }

            return value;
        }

        protected internal virtual void CheckGetTaskVariable(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadTask(task);
        }
    }
}