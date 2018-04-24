using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using BatchEntity = ESS.FW.Bpm.Engine.Batch.Impl.BatchEntity;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;

namespace ESS.FW.Bpm.Engine.Impl.migration.batch
{

    public class MigrateProcessInstanceBatchCmd : AbstractMigrationCmd<IBatch>
    {
        protected internal static readonly MigrationLogger Logger = ProcessEngineLogger.MigrationLogger;

        public MigrateProcessInstanceBatchCmd(MigrationPlanExecutionBuilderImpl migrationPlanExecutionBuilder)
            : base(migrationPlanExecutionBuilder)
        {
        }

        public override IBatch Execute(CommandContext commandContext)
        {
            var migrationPlan = ExecutionBuilder.MigrationPlan;
            var processInstanceIds = CollectProcessInstanceIds(commandContext);

            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "Migration plan cannot be null", "migration plan",
                migrationPlan);
            //EnsureUtil.EnsureNotEmpty(typeof (BadUserRequestException), "Process instance ids cannot empty",
            //    "process instance ids", processInstanceIds);
            EnsureUtil.EnsureNotContainsNull(typeof(BadUserRequestException), "Process instance ids cannot be null",
                "process instance ids", processInstanceIds);

            var sourceProcessDefinition = ResolveSourceProcessDefinition(commandContext);
            var targetProcessDefinition = ResolveTargetProcessDefinition(commandContext);

            base.CheckAuthorizations(commandContext, sourceProcessDefinition, targetProcessDefinition, processInstanceIds);
            WriteUserOperationLog(commandContext, sourceProcessDefinition, targetProcessDefinition,
                processInstanceIds.Count, true);

            var batch = CreateBatch(commandContext, migrationPlan, processInstanceIds, sourceProcessDefinition);

            batch.CreateSeedJobDefinition();
            batch.CreateMonitorJobDefinition();
            batch.CreateBatchJobDefinition();

            batch.FireHistoricStartEvent();

            batch.CreateSeedJob();

            return batch;
        }

        protected internal override void CheckAuthorizations(CommandContext commandContext,
            ProcessDefinitionEntity sourceDefinition, ProcessDefinitionEntity targetDefinition,
            ICollection<string> processInstanceIds)
        {
            //commandContext.AuthorizationManager.checkAuthorization(Permissions.CREATE, Resources.BATCH);

            base.CheckAuthorizations(commandContext, sourceDefinition, targetDefinition, processInstanceIds);
        }

        protected internal virtual BatchEntity CreateBatch(CommandContext commandContext, IMigrationPlan migrationPlan,
            ICollection<string> processInstanceIds, ProcessDefinitionEntity sourceProcessDefinition)
        {
            var processEngineConfiguration = commandContext.ProcessEngineConfiguration;
            var batchJobHandler = getBatchJobHandler(processEngineConfiguration);

            var configuration = new MigrationBatchConfiguration(new List<string>(processInstanceIds), migrationPlan,
                ExecutionBuilder.SkipCustomListenersRenamed, ExecutionBuilder.SkipIoMappingsRenamed);

            var batch = new BatchEntity();
            batch.Type = batchJobHandler.Type;
            batch.TotalJobs = CalculateSize(processEngineConfiguration, configuration);
            batch.BatchJobsPerSeed = processEngineConfiguration.BatchJobsPerSeed;
            batch.InvocationsPerBatchJob = processEngineConfiguration.InvocationsPerBatchJob;
            batch.ConfigurationBytes = batchJobHandler.WriteConfiguration(configuration);
            batch.TenantId = sourceProcessDefinition.TenantId;
            (commandContext.BatchManager).Add(batch);

            return batch;
        }

        protected internal virtual int CalculateSize(ProcessEngineConfigurationImpl engineConfiguration,
            MigrationBatchConfiguration batchConfiguration)
        {
            double invocationsPerBatchJob = engineConfiguration.InvocationsPerBatchJob;
            double processInstanceCount = batchConfiguration.Ids.Count;

            return (int) Math.Ceiling(processInstanceCount / invocationsPerBatchJob);
        }

        protected internal virtual IBatchJobHandler getBatchJobHandler(
            ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            return
                    processEngineConfiguration.BatchHandlers[BatchFields.TypeProcessInstanceMigration];
            return null;
        }
    }
}