using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     <seealso cref="IJobHandler{T}" /> implementation for timer start events which are embedded into an event subprocess.
    ///     The configuration is the id of the start event activity.
    ///     
    ///     
    /// </summary>
    public class TimerStartEventSubprocessJobHandler : TimerEventJobHandler
    {
        public const string TYPE = "timer-start-event-subprocess";

        public override string Type
        {
            get { return TYPE; }
        }

        public override void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            var activityId = ((TimerJobConfiguration)configuration).TimerElementKey;
            ActivityImpl eventSubprocessActivity = execution.ProcessDefinition.FindActivity(activityId) as ActivityImpl;

            if (eventSubprocessActivity != null)
            {
                execution.ExecuteEventHandlerActivity(eventSubprocessActivity);
            }
            else
            {
                throw new ProcessEngineException(
                    "Error while triggering event subprocess using timer start event: cannot find activity with id '" +
                    configuration + "'.");
            }
        }

        public override void Execute<T>(T configuration, ExecutionEntity execution, CommandContext commandContext,
            string tenantId)
        {
            throw new NotImplementedException();
        }

        public override void OnDelete<T>(T configuration, JobEntity jobEntity)
        {
            throw new NotImplementedException();
        }
    }
}