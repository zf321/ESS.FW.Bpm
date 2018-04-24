using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    /// <summary>
    /// 
    /// </summary>
    public class CreateIncidentCmd : ICommand<IIncident>
    {

        protected internal string incidentType;
        protected internal string executionId;
        protected internal string configuration;
        protected internal string message;

        public CreateIncidentCmd(string incidentType, string executionId, string configuration, string message)
        {
            this.incidentType = incidentType;
            this.executionId = executionId;
            this.configuration = configuration;
            this.message = message;
        }

        public IIncident Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Execution id cannot be null", "executionId", executionId);
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "incidentType", value:incidentType);

            ExecutionEntity execution = commandContext.ExecutionManager.FindExecutionById(executionId);
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Cannot find an execution with executionId '" + executionId + "'", "execution", execution);
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Execution must be related to an activity", "activity", execution.Activity);

            foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckUpdateProcessInstance(execution);
            }

            return execution.CreateIncident(incidentType, configuration, message);
        }
    }
}
