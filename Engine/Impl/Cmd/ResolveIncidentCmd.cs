using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public class ResolveIncidentCmd : ICommand<object>
    {
        private readonly string _incidentId;

        public ResolveIncidentCmd(string incidentId)
        {
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "", "incidentId", incidentId);
            this._incidentId = incidentId;
        }

        public object Execute(CommandContext commandContext)
        {
            IIncident incident = commandContext.IncidentManager.FindIncidentById(_incidentId);

            EnsureUtil.EnsureNotNull(typeof(NotFoundException), "Cannot find an incident with id '" + _incidentId + "'",
                "incident", incident);

            if (incident.IncidentType.Equals("failedJob") || incident.IncidentType.Equals("failedExternalTask"))
                throw new BadUserRequestException("Cannot resolve an incident of type " + incident.IncidentType);

            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "", "executionId", incident.ExecutionId);
            var execution = commandContext.ExecutionManager.FindExecutionById(incident.ExecutionId);

            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException),
                "Cannot find an execution for an incident with id '" + _incidentId + "'", "execution", execution);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckUpdateProcessInstance(execution);

            execution.ResolveIncident(_incidentId);
            return null;
        }
    }
}