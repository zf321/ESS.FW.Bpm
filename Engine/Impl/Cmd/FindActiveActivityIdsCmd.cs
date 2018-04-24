using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class FindActiveActivityIdsCmd : ICommand<IList<string>>
    {
        private const long SerialVersionUid = 1L;
        protected internal string ExecutionId;

        public FindActiveActivityIdsCmd(string executionId)
        {
            this.ExecutionId = executionId;
        }

        public virtual IList<string> Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("executionId", ExecutionId);

            // fetch execution
            IExecutionManager executionManager = commandContext.ExecutionManager;
            ExecutionEntity execution = executionManager.FindExecutionById(ExecutionId);
            EnsureUtil.EnsureNotNull("execution " + ExecutionId + " doesn't exist", "execution", execution);

            CheckGetActivityIds(execution, commandContext);

            // fetch active activities
            return execution.FindActiveActivityIds();
        }

        protected internal virtual void CheckGetActivityIds(ExecutionEntity execution, CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckReadProcessInstance(execution);
        }
    }
}