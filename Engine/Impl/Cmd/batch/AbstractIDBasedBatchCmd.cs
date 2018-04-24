using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd.Batch
{

    /// <summary>
    ///     Representation of common logic to all Batch commands which are based on list of
    ///     IDs.
    ///     
    /// </summary>
    public abstract class AbstractIdBasedBatchCmd<T> : AbstractBatchCmd<T> 
    {
        protected internal virtual BatchEntity CreateBatch(CommandContext commandContext, IList<string> ids)
        {
            var processEngineConfiguration = commandContext.ProcessEngineConfiguration;
            var batchJobHandler = GetBatchJobHandler(processEngineConfiguration);

            var configuration = GetAbstractIdsBatchConfiguration(ids);

            var batch = new BatchEntity();
            batch.Type = batchJobHandler.Type;
            batch.TotalJobs = CalculateSize(processEngineConfiguration, configuration);
            batch.BatchJobsPerSeed = processEngineConfiguration.BatchJobsPerSeed;
            batch.InvocationsPerBatchJob = processEngineConfiguration.InvocationsPerBatchJob;
            batch.ConfigurationBytes = batchJobHandler.WriteConfiguration(configuration);
            (commandContext.BatchManager as BatchManager).Add(batch);

            return batch;
        }

        protected internal virtual int CalculateSize(ProcessEngineConfigurationImpl engineConfiguration,
            BatchConfiguration batchConfiguration)
        {
            var invocationsPerBatchJob = engineConfiguration.InvocationsPerBatchJob;
            var processInstanceCount = batchConfiguration.Ids.Count;

            return (int) Math.Ceiling((decimal) (processInstanceCount/invocationsPerBatchJob));
        }

        protected internal abstract BatchConfiguration GetAbstractIdsBatchConfiguration(IList<string> processInstanceIds);

        protected internal abstract IBatchJobHandler GetBatchJobHandler(
            ProcessEngineConfigurationImpl processEngineConfiguration);
    }
}