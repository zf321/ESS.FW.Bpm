using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.migration.batch
{
    /// <summary>
    ///     Job handler for batch migration jobs. The batch migration job
    ///     migrates a list of process instances.
    /// </summary>
    public class MigrationBatchJobHandler : AbstractBatchJobHandler<MigrationBatchConfiguration>
    {
        public static readonly BatchJobDeclaration JOB_DECLARATION =
            new BatchJobDeclaration(BatchFields.TypeProcessInstanceMigration);

        public override string Type
        {
            get { return BatchFields.TypeProcessInstanceMigration; }
        }

        public override IJobDeclaration/* JobDeclaration<MessageEntity> */JobDeclaration
        {
            get { return JOB_DECLARATION; }
        }

        //protected internal override MigrationBatchConfigurationJsonConverter JsonConverterInstance
        //{
        // get
        // {
        //return MigrationBatchConfigurationJsonConverter.INSTANCE;
        // }
        //}

        protected internal override IJobHandlerConfiguration CreateJobConfiguration(
            IJobHandlerConfiguration configuration, IList<string> processIdsForJob)
        {
            var config = (MigrationBatchConfiguration)configuration;
            return new MigrationBatchConfiguration(processIdsForJob, config.MigrationPlan,
                config.SkipCustomListeners, config.SkipIoMappings);
        }

        protected internal override void PostProcessJob(IJobHandlerConfiguration configuration, JobEntity job)
        {
            var config = (MigrationBatchConfiguration)configuration;
            var commandContext = Context.CommandContext;
            var sourceProcessDefinitionId = config.MigrationPlan.SourceProcessDefinitionId;

            var processDefinition = GetProcessDefinition(commandContext, sourceProcessDefinitionId);
            job.DeploymentId = processDefinition.DeploymentId;
        }

        public void Execute(BatchJobConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            //ResourceEntity configurationEntity = commandContext.DbEntityManager.selectById(typeof(ResourceEntity), configuration.ConfigurationByteArrayId);

            //MigrationBatchConfiguration batchConfiguration = readConfiguration(configurationEntity.Bytes);

            //MigrationPlanExecutionBuilder executionBuilder = commandContext.ProcessEngineConfiguration.RuntimeService.newMigration(batchConfiguration.MigrationPlan).processInstanceIds(batchConfiguration.Ids);

            //if (batchConfiguration.SkipCustomListeners)
            //{
            //  executionBuilder.skipCustomListeners();
            //}
            //if (batchConfiguration.SkipIoMappings)
            //{
            //  executionBuilder.skipIoMappings();
            //}

            // uses internal API in order to skip writing user operation log (CommandContext#disableUserOperationLog
            // is not sufficient with legacy engine config setting "restrictUserOperationLogToAuthenticatedUsers" = false)
            //((MigrationPlanExecutionBuilderImpl) executionBuilder).execute(false);

            //commandContext.ByteArrayManager.delete(configurationEntity);
        }

        protected internal virtual ProcessDefinitionEntity GetProcessDefinition(CommandContext commandContext,
            string processDefinitionId)
        {
            //return commandContext.ProcessEngineConfiguration.DeploymentCache.findDeployedProcessDefinitionById(processDefinitionId);
            return null;
        }
    }
}