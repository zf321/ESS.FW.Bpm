using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    public class TimerStartEventJobHandler : TimerEventJobHandler
    {
        public const string TYPE = "timer-start-event";

        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;

        public override string Type
        {
            get { return TYPE; }
        }

        public override void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;

            var definitionKey = ((TimerJobConfiguration)configuration).TimerElementKey;
            IProcessDefinition processDefinition =
                deploymentCache.FindDeployedLatestProcessDefinitionByKeyAndTenantId(definitionKey, tenantId);

            try
            {
                StartProcessInstance(commandContext, tenantId, processDefinition);
            }
            catch (System.Exception e)
            {
                throw e;
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

        protected internal virtual void StartProcessInstance(CommandContext commandContext, string tenantId,
            IProcessDefinition processDefinition)
        {
            if (!processDefinition.Suspended)
            {
                var runtimeService = commandContext.ProcessEngineConfiguration.RuntimeService;
                runtimeService.CreateProcessInstanceByKey(processDefinition.Key)
                    .SetProcessDefinitionTenantId(tenantId)
                    .Execute();
            }
            else
            {
                Log.IgnoringSuspendedJob(processDefinition);
            }
        }
    }
}