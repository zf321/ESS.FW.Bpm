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
    public class GetExecutionVariableCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal string ExecutionId;
        protected internal bool IsLocal;
        protected internal string VariableName;

        public GetExecutionVariableCmd(string executionId, string variableName, bool isLocal)
        {
            this.ExecutionId = executionId;
            this.VariableName = variableName;
            this.IsLocal = isLocal;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("executionId", ExecutionId);
            EnsureUtil.EnsureNotNull("variableName", VariableName);

            ExecutionEntity execution = commandContext.ExecutionManager.FindExecutionById(ExecutionId);

            EnsureUtil.EnsureNotNull("execution " + ExecutionId + " doesn't exist", "execution", execution);

            CheckGetExecutionVariable(execution, commandContext);

            var value = new object();

            if (IsLocal)
            {
                value = execution.GetVariableLocal(VariableName, true);
            }
            else
            {
                value = execution.GetVariable(VariableName, true);
            }

            return value;
        }

        protected internal virtual void CheckGetExecutionVariable(ExecutionEntity execution,
            CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadProcessInstance(execution);
        }
    }
}