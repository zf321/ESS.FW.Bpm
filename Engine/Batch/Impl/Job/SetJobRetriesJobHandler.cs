using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;

namespace ESS.FW.Bpm.Engine.Batch.Impl.Job
{
    /// <summary>
    ///     
    /// </summary>
    public class SetJobRetriesJobHandler : AbstractBatchJobHandler<SetJobRetriesBatchConfiguration>
    {
        public static readonly BatchJobDeclaration jobDeclaration =
            new BatchJobDeclaration(BatchFields.TypeSetJobRetries);

        public override string Type
        {
            get { return BatchFields.TypeSetJobRetries; }
        }

        //protected internal override SetJobRetriesBatchConfigurationJsonConverter JsonConverterInstance
        //{
        //    get { return SetJobRetriesBatchConfigurationJsonConverter.INSTANCE; }
        //}

        public override IJobDeclaration/* JobDeclaration<BatchJobContext, MessageEntity> */JobDeclaration
        {
            get { return jobDeclaration; }
        }

        protected internal override IJobHandlerConfiguration CreateJobConfiguration(
            IJobHandlerConfiguration configuration, IList<string> jobIds)
        {
            return new SetJobRetriesBatchConfiguration(jobIds, ((SetJobRetriesBatchConfiguration)configuration).Retries);
        }

        //public override void execute(BatchJobConfiguration configuration, ExecutionEntity execution,
        //    CommandContext commandContext, string tenantId)

        //{
        //    ResourceEntity configurationEntity = commandContext.Get(typeof (ResourceEntity),
        //        configuration.ConfigurationByteArrayId);

        //    var batchConfiguration = readConfiguration(configurationEntity.Bytes);

        //    var initialLegacyRestrictions = commandContext.RestrictUserOperationLogToAuthenticatedUsers;
        //    commandContext.disableUserOperationLog();
        //    commandContext.RestrictUserOperationLogToAuthenticatedUsers = true;
        //    try
        //    {
        //        commandContext.ProcessEngineConfiguration.ManagementService.setJobRetries(batchConfiguration.Ids,
        //            batchConfiguration.Retries);
        //    }
        //    finally
        //    {
        //        commandContext.enableUserOperationLog();
        //        commandContext.RestrictUserOperationLogToAuthenticatedUsers = initialLegacyRestrictions;
        //    }

        //    //commandContext.ByteArrayManager.delete(configurationEntity);
        //}
    }
}