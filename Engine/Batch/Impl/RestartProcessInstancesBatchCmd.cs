using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Batch.Impl
{
    
    /// 
    /// <summary>
    /// 
    /// </summary>
    public class RestartProcessInstancesBatchCmd : AbstractRestartProcessInstanceCmd<IBatch>
    {

        private readonly CommandLogger LOG = ProcessEngineLogger.CmdLogger;

        public RestartProcessInstancesBatchCmd(ICommandExecutor commandExecutor, RestartProcessInstanceBuilderImpl builder) : base(commandExecutor, builder)
        {
        }

        public override IBatch Execute(CommandContext commandContext)
        {
            IList<AbstractProcessInstanceModificationCommand> instructions = builder.Instructions;
            ICollection<string> processInstanceIds = CollectProcessInstanceIds();

            EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "Restart instructions cannot be empty", "instructions", instructions);
            EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "Process instance ids cannot be empty", "processInstanceIds", processInstanceIds);
            EnsureUtil.EnsureNotContainsNull(typeof(BadUserRequestException), "Process instance ids cannot be null", "processInstanceIds", processInstanceIds);

            commandContext.AuthorizationManager.CheckAuthorization(Permissions.Create, Resources.Batch);
            ProcessDefinitionEntity processDefinition = GetProcessDefinition(commandContext, builder.ProcessDefinitionId);

            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Process definition cannot be null", processDefinition);
            EnsureTenantAuthorized(commandContext, processDefinition);

            WriteUserOperationLog(commandContext, processDefinition, processInstanceIds.Count, true);

            List<string> ids = new List<string>();
            ids.AddRange(processInstanceIds);
            BatchEntity batch = CreateBatch(commandContext, instructions, ids, processDefinition);
            batch.CreateSeedJobDefinition();
            batch.CreateMonitorJobDefinition();
            batch.CreateBatchJobDefinition();

            batch.FireHistoricStartEvent();

            batch.CreateSeedJob();
            return batch;

        }

        protected internal virtual BatchEntity CreateBatch(CommandContext commandContext, IList<AbstractProcessInstanceModificationCommand> instructions, IList<string> processInstanceIds, ProcessDefinitionEntity processDefinition)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration = commandContext.ProcessEngineConfiguration;
            IBatchJobHandler batchJobHandler = GetBatchJobHandler(processEngineConfiguration);

            RestartProcessInstancesBatchConfiguration configuration = new RestartProcessInstancesBatchConfiguration(processInstanceIds, instructions, builder.ProcessDefinitionId, builder.InitialVariables, builder.SkipCustomListeners, builder.SkipIoMappings, builder.WithoutBusinessKey);

            BatchEntity batch = new BatchEntity();
            batch.Type = batchJobHandler.Type;
            batch.TotalJobs = CalculateSize(processEngineConfiguration, configuration);
            batch.BatchJobsPerSeed = processEngineConfiguration.BatchJobsPerSeed;
            batch.InvocationsPerBatchJob = processEngineConfiguration.InvocationsPerBatchJob;
            batch.ConfigurationBytes = batchJobHandler.WriteConfiguration(configuration);
            batch.TenantId = processDefinition.TenantId;
            commandContext.BatchManager.Add(batch);

            return batch;
        }

        protected internal virtual int CalculateSize(ProcessEngineConfigurationImpl engineConfiguration, RestartProcessInstancesBatchConfiguration batchConfiguration)
        {
            int invocationsPerBatchJob = engineConfiguration.InvocationsPerBatchJob;
            int processInstanceCount = batchConfiguration.Ids.Count;

            return (int)Math.Ceiling((decimal) (processInstanceCount / invocationsPerBatchJob));
        }
        
        protected internal virtual IBatchJobHandler GetBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            IDictionary<string, IBatchJobHandler> batchHandlers = processEngineConfiguration.BatchHandlers;
            return batchHandlers[BatchFields.TypeProcessInstanceRestart];
        }

        protected internal virtual void EnsureTenantAuthorized(CommandContext commandContext, ProcessDefinitionEntity processDefinition)
        {
            if (!commandContext.TenantManager.IsAuthenticatedTenant(processDefinition.TenantId))
            {
                throw LOG.ExceptionCommandWithUnauthorizedTenant("restart process instances of process definition '" + processDefinition.Id + "'");
            }
        }
    }
}
