using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    
    public class ProcessInstanceModificationBatchCmd : AbstractModificationCmd<IBatch>
    {

        protected internal static readonly CommandLogger Logger = ProcessEngineLogger.CmdLogger;

        public ProcessInstanceModificationBatchCmd(ModificationBuilderImpl modificationBuilderImpl) : base(modificationBuilderImpl)
        {
        }

        public override IBatch Execute(CommandContext commandContext)
        {
            IList<AbstractProcessInstanceModificationCommand> instructions = builder.Instructions;
            ICollection<string> processInstanceIds = CollectProcessInstanceIds(commandContext);

            EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "Modification instructions cannot be empty", instructions);
            EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "Process instance ids cannot be empty", "Process instance ids", processInstanceIds);
            EnsureUtil.EnsureNotContainsNull(typeof(BadUserRequestException), "Process instance ids cannot be null", "Process instance ids", processInstanceIds);

            commandContext.AuthorizationManager.CheckAuthorization(Permissions.Create, Resources.Batch);

            ProcessDefinitionEntity processDefinition = GetProcessDefinition(commandContext, builder.ProcessDefinitionId);
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Process definition id cannot be null", processDefinition);

            WriteUserOperationLog(commandContext, processDefinition, processInstanceIds.Count, true);

            BatchEntity batch = CreateBatch(commandContext, instructions, processInstanceIds, processDefinition);
            batch.CreateSeedJobDefinition();
            batch.CreateMonitorJobDefinition();
            batch.CreateBatchJobDefinition();

            batch.FireHistoricStartEvent();

            batch.CreateSeedJob();
            return batch;
        }

        protected internal virtual BatchEntity CreateBatch(CommandContext commandContext, IList<AbstractProcessInstanceModificationCommand> instructions, ICollection<string> processInstanceIds, ProcessDefinitionEntity processDefinition)
        {

            ProcessEngineConfigurationImpl processEngineConfiguration = commandContext.ProcessEngineConfiguration;
            IBatchJobHandler batchJobHandler = GetBatchJobHandler(processEngineConfiguration);

            ModificationBatchConfiguration configuration = new ModificationBatchConfiguration(new List<string>(processInstanceIds), builder.ProcessDefinitionId, instructions, builder.SkipCustomListeners, builder.SkipIoMappings);

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

        protected internal virtual int CalculateSize(ProcessEngineConfigurationImpl engineConfiguration, ModificationBatchConfiguration batchConfiguration)
        {
            int invocationsPerBatchJob = engineConfiguration.InvocationsPerBatchJob;
            int processInstanceCount = batchConfiguration.Ids.Count;

            return (int) Math.Ceiling((double) (processInstanceCount / invocationsPerBatchJob));
        }
        
        protected internal virtual IBatchJobHandler GetBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            IDictionary<string, IBatchJobHandler> batchHandlers = processEngineConfiguration.BatchHandlers;
            return batchHandlers[BatchFields.TypeProcessInstanceModification];
        }

    }
}
