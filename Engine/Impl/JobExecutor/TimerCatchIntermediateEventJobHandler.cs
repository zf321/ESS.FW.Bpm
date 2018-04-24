using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    public class TimerCatchIntermediateEventJobHandler : TimerEventJobHandler
    {
        public const string TYPE = "timer-intermediate-transition";

        public override string Type
        {
            get { return TYPE; }
        }

        public override void Execute(IJobHandlerConfiguration _configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            TimerJobConfiguration configuration = _configuration as TimerJobConfiguration;
            var activityId = configuration.TimerElementKey;
            ActivityImpl intermediateEventActivity = execution.GetProcessDefinition().FindActivity(activityId) as ActivityImpl;

            EnsureUtil.EnsureNotNull(
                "Error while firing timer: intermediate event activity " + configuration + " not found",
                "intermediateEventActivity", intermediateEventActivity);

            try
            {
                if (activityId.Equals(execution.ActivityId))
                {
                    // Regular Intermediate timer catch
                    execution.Signal("signal", null);
                }
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException("exception during timer execution: " + e.Message, e);
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