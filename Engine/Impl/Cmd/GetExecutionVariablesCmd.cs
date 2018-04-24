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
    ///     
    /// </summary>
    [Serializable]
    public class GetExecutionVariablesCmd : ICommand<IVariableMap>
    {
        private const long SerialVersionUid = 1L;
        protected internal bool DeserializeValues;
        protected internal string ExecutionId;
        protected internal bool IsLocal;
        protected internal ICollection<string> VariableNames;

        public GetExecutionVariablesCmd(string executionId, ICollection<string> variableNames, bool isLocal,
            bool deserializeValues)
        {
            this.ExecutionId = executionId;
            this.VariableNames = variableNames;
            this.IsLocal = isLocal;
            this.DeserializeValues = deserializeValues;
        }

        public virtual IVariableMap Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("executionId", ExecutionId);

            ExecutionEntity execution = commandContext.ExecutionManager.FindExecutionById(ExecutionId);

            EnsureUtil.EnsureNotNull("execution " + ExecutionId + " doesn't exist", "execution", execution);

            CheckGetExecutionVariables(execution, commandContext);

            var executionVariables = new VariableMapImpl();

            //collect variables from execution
           execution.CollectVariables(executionVariables, VariableNames, IsLocal, DeserializeValues);

            return executionVariables;
        }

        protected internal virtual void CheckGetExecutionVariables(ExecutionEntity execution,
            CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadProcessInstance(execution);
        }
    }
}