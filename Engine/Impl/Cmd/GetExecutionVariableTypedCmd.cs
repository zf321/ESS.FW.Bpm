using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetExecutionVariableTypedCmd<T> : ICommand<T>
    {
        private const long SerialVersionUid = 1L;
        protected internal bool DeserializeValue;
        protected internal string ExecutionId;
        protected internal bool IsLocal;
        protected internal string VariableName;

        public GetExecutionVariableTypedCmd(string executionId, string variableName, bool isLocal, bool deserializeValue)
        {
            this.ExecutionId = executionId;
            this.VariableName = variableName;
            this.IsLocal = isLocal;
            this.DeserializeValue = deserializeValue;
        }

        public virtual T Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("executionId", ExecutionId);
            EnsureUtil.EnsureNotNull("variableName", VariableName);

            ExecutionEntity execution = commandContext.ExecutionManager.FindExecutionById(ExecutionId);

            EnsureUtil.EnsureNotNull("execution " + ExecutionId + " doesn't exist", "execution", execution);

            CheckGetExecutionVariableTyped(execution, commandContext);

            T value ;

            if (IsLocal)
            {
                value = execution.GetVariableLocalTyped<T>(VariableName, DeserializeValue);
            }
            else
            {
                value= execution.GetVariableTyped<T>(VariableName, DeserializeValue);
            }

            return value;
        }

        public virtual void CheckGetExecutionVariableTyped(ExecutionEntity execution, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadProcessInstance(execution);
        }
    }
}