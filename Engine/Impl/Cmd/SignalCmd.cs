using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class SignalCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal readonly IDictionary<string, object> ProcessVariables;

        protected internal string ExecutionId;
        protected internal object SignalData;
        protected internal string SignalName;

        public SignalCmd(string executionId, string signalName, object signalData,
            IDictionary<string, object> processVariables)
        {
            this.ExecutionId = executionId;
            this.SignalName = signalName;
            this.SignalData = signalData;
            this.ProcessVariables = processVariables;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "executionId is null", "executionId", ExecutionId);

            ExecutionEntity execution = commandContext.ExecutionManager.FindExecutionById(ExecutionId);
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "execution " + ExecutionId + " doesn't exist", "execution", execution);

            foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckUpdateProcessInstance(execution);
            }

            if (ProcessVariables != null)
            {
                execution.Variables = ProcessVariables;
            }

            execution.Signal(SignalName, SignalData);
            return null;
        }
    }
}