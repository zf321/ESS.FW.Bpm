using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Batch.Impl.Deletion
{
    /// <summary>
    ///     
    /// </summary>
    public class DeleteProcessInstancesJobHandler : AbstractBatchJobHandler<DeleteProcessInstanceBatchConfiguration>
    {
        public static readonly BatchJobDeclaration JOB_DECLARATION =
            new BatchJobDeclaration(BatchFields.TypeProcessInstanceDeletion);

        public override string Type
        {
            get { return BatchFields.TypeProcessInstanceDeletion; }
        }

        //protected internal override DeleteProcessInstanceBatchConfigurationJsonConverter JsonConverterInstance
        //{
        //    get { return DeleteProcessInstanceBatchConfigurationJsonConverter.INSTANCE; }
        //}

        public override IJobDeclaration/*<MessageEntity> */JobDeclaration
        {
            get { return JOB_DECLARATION; }
        }

        //protected internal override DeleteHistoricProcessInstanceBatchConfigurationJsonConverter JsonConverterInstance
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        protected internal override IJobHandlerConfiguration CreateJobConfiguration(
            IJobHandlerConfiguration configuration, IList<string> processIdsForJob)
        {
            return new DeleteProcessInstanceBatchConfiguration(processIdsForJob, ((DeleteProcessInstanceBatchConfiguration)configuration).DeleteReason);
        }

        public override void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            throw new System.NotImplementedException();
            //ResourceEntity configurationEntity = commandContext.Get(typeof(ResourceEntity),
            //    configuration.ConfigurationByteArrayId);

            //var batchConfiguration = readConfiguration(configurationEntity.Bytes);

            //var initialLegacyRestrictions = commandContext.RestrictUserOperationLogToAuthenticatedUsers;
            //commandContext.disableUserOperationLog();
            //commandContext.RestrictUserOperationLogToAuthenticatedUsers = true;
            //try
            //{
            //    commandContext.ProcessEngineConfiguration.RuntimeService.deleteProcessInstances(batchConfiguration.Ids,
            //        batchConfiguration.deleteReason, false, true);
            //}
            //finally
            //{
            //    commandContext.enableUserOperationLog();
            //    commandContext.RestrictUserOperationLogToAuthenticatedUsers = initialLegacyRestrictions;
            //}

            //commandContext.ByteArrayManager.delete(configurationEntity);
        }
    }
}