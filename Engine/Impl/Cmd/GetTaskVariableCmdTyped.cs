using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    


    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetTaskVariableCmdTyped : ICommand<ITypedValue>
    {
        private const long SerialVersionUid = 1L;
        protected internal bool DeserializeValue;
        protected internal bool IsLocal;
        protected internal string TaskId;
        protected internal string VariableName;

        public GetTaskVariableCmdTyped(string taskId, string variableName, bool isLocal, bool deserializeValue)
        {
            this.TaskId = taskId;
            this.VariableName = variableName;
            this.IsLocal = isLocal;
            this.DeserializeValue = deserializeValue;
        }

        public virtual ITypedValue Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("taskId", TaskId);
            EnsureUtil.EnsureNotNull("variableName", VariableName);

            TaskEntity task = context.Impl.Context.CommandContext.TaskManager.FindTaskById(TaskId);

            EnsureUtil.EnsureNotNull("ITask " + TaskId + " doesn't exist", "ITask", task);

            CheckGetTaskVariableTyped(task, commandContext);

            ITypedValue value;

            if (IsLocal)
            {
                value = task.GetVariableLocalTyped< ITypedValue>(VariableName, DeserializeValue);
            }
            else
            {
                value = task.GetVariableTyped< ITypedValue>(VariableName, DeserializeValue);
            }

            return value;
        }

        protected internal virtual void CheckGetTaskVariableTyped(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadTask(task);
        }
    }
}