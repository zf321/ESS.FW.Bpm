using System;
using System.Collections.Generic;


using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    


    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetTaskVariablesCmd : ICommand<IVariableMap>
    {
        private const long SerialVersionUid = 1L;
        protected internal bool DeserializeValues;
        protected internal bool IsLocal;
        protected internal string TaskId;
        protected internal ICollection<string> VariableNames;

        public GetTaskVariablesCmd(string taskId, ICollection<string> variableNames, bool isLocal,
            bool deserializeValues)
        {
            this.TaskId = taskId;
            this.VariableNames = variableNames;
            this.IsLocal = isLocal;
            this.DeserializeValues = deserializeValues;
        }

        public virtual IVariableMap Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("taskId", TaskId);

            TaskEntity task = context.Impl.Context.CommandContext.TaskManager.FindTaskById(TaskId);

            EnsureUtil.EnsureNotNull("ITask " + TaskId + " doesn't exist", "ITask", task);

            CheckGetTaskVariables(task, commandContext);

            var variables = new VariableMapImpl();

            //collect variables from ITask
           task.CollectVariables(variables, VariableNames, IsLocal, DeserializeValues);

            return variables;
        }

        protected internal virtual void CheckGetTaskVariables(TaskEntity task, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadTask(task);
        }
    }
}