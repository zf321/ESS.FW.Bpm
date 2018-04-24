using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Batch.Impl.Deletion
{

    /// <summary>
    ///     
    /// </summary>
    public class DeleteHistoricProcessInstancesJobHandler : AbstractBatchJobHandler<BatchConfiguration>
    {
        public static readonly BatchJobDeclaration JOB_DECLARATION =
            new BatchJobDeclaration(BatchFields.TypeHistoricProcessInstanceDeletion);

        public override string Type
        {
            get { return BatchFields.TypeHistoricProcessInstanceDeletion; }
        }

        //protected internal override DeleteHistoricProcessInstanceBatchConfigurationJsonConverter JsonConverterInstance
        //{
        //    get { return DeleteHistoricProcessInstanceBatchConfigurationJsonConverter.INSTANCE; }
        //}

        public override IJobDeclaration/* JobDeclaration<MessageEntity> */JobDeclaration
        {
            get { return JOB_DECLARATION; }
        }

        protected internal override IJobHandlerConfiguration CreateJobConfiguration(IJobHandlerConfiguration jobHandlerConfiguration,
            IList<string> processIdsForJob)
        {
            return new BatchConfiguration(processIdsForJob);
        }

        public void Execute(BatchJobConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            throw new System.NotImplementedException();
            //ResourceEntity configurationEntity = commandContext.Get(typeof(ResourceEntity), configuration.ConfigurationByteArrayId);

            //BatchConfiguration batchConfiguration = readConfiguration(configurationEntity.Bytes);

            var initialLegacyRestrictions = commandContext.RestrictUserOperationLogToAuthenticatedUsers;
            commandContext.DisableUserOperationLog();
            commandContext.RestrictUserOperationLogToAuthenticatedUsers = true;
            try
            {
                //commandContext.ProcessEngineConfiguration.HistoryService.deleteHistoricProcessInstances(batchConfiguration.Ids);
            }
            finally
            {
                commandContext.EnableUserOperationLog();
                commandContext.RestrictUserOperationLogToAuthenticatedUsers = initialLegacyRestrictions;
            }

            //commandContext.ByteArrayManager.delete(configurationEntity);
        }
    }
}