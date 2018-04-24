using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Batch.Impl.Job;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd.Batch;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public abstract class AbstractSetJobsRetriesBatchCmd : AbstractIdBasedBatchCmd<IBatch>
    {
        protected internal int Retries;

        public override IBatch Execute(CommandContext commandContext)
        {
            var jobIds = CollectJobIds(commandContext);

            //EnsureUtil.EnsureNotEmpty(typeof(BadUserRequestException), "jobIds", jobIds);
            EnsureUtil.EnsureGreaterThanOrEqual("Retries count", Retries, 0);
            CheckAuthorizations(commandContext);
            WriteUserOperationLog(commandContext, Retries, jobIds.Count, true);

            var batch = CreateBatch(commandContext, jobIds);
            batch.CreateSeedJobDefinition();
            batch.CreateMonitorJobDefinition();
            batch.CreateBatchJobDefinition();

            batch.FireHistoricStartEvent();

            batch.CreateSeedJob();

            return batch;
        }


        protected internal virtual void WriteUserOperationLog(CommandContext commandContext, int retries,
            int numInstances, bool async)
        {
            IList<PropertyChange> propertyChanges = new List<PropertyChange>();
            propertyChanges.Add(new PropertyChange("nrOfInstances", null, numInstances));
            propertyChanges.Add(new PropertyChange("async", null, async));
            propertyChanges.Add(new PropertyChange("retries", null, retries));

            commandContext.OperationLogManager.LogProcessInstanceOperation(
                UserOperationLogEntryFields.OperationTypeSetJobRetries, null, null, null, propertyChanges);
        }

        protected internal abstract IList<string> CollectJobIds(CommandContext commandContext);

        protected internal override BatchConfiguration GetAbstractIdsBatchConfiguration(IList<string> processInstanceIds)
        {
            return new SetJobRetriesBatchConfiguration(processInstanceIds, Retries);
        }


        protected internal override IBatchJobHandler GetBatchJobHandler(
            ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            return processEngineConfiguration.BatchHandlers[BatchFields.TypeSetJobRetries];
        }
    }
}