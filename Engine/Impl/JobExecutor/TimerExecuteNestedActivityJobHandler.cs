using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.engine.impl.Util.EnsureUtil.ensureNotNull;


    /// <summary>
    ///      
    ///     
    /// </summary>
    public class TimerExecuteNestedActivityJobHandler : TimerEventJobHandler
    {
        public const string TYPE = "timer-transition";

        public override string Type
        {
            get { return TYPE; }
        }

        public override void Execute(IJobHandlerConfiguration _configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            TimerJobConfiguration configuration = _configuration as TimerJobConfiguration;
            var activityId = configuration.TimerElementKey;
            ActivityImpl activity = execution.GetProcessDefinition().FindActivity(activityId) as ActivityImpl;

            EnsureUtil.EnsureNotNull(
                "Error while firing timer: boundary event activity " + configuration + " not found",
                "boundary event activity", activity);

            try
            {
                execution.ExecuteEventHandlerActivity(activity);
            }
            //catch (Exception e)
            //{
            //    throw e;
            //}
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