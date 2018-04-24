using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Batch.Impl
{


    /// <summary>
    ///     The batch seed job handler is responsible to
    ///     create all jobs to be executed by the batch.
    ///     If all jobs are created a seed monitor job is
    ///     created to oversee the completion of the batch
    ///     (see <seealso cref="BatchMonitorJobHandler" />).
    /// </summary>
    public class BatchSeedJobHandler : IJobHandler<BatchSeedJobHandler.BatchSeedJobConfiguration>
    {
        public const string TYPE = "batch-seed-job";

        public virtual string Type
        {
            get { return TYPE; }
        }

        public virtual IJobHandlerConfiguration NewConfiguration(string canonicalString)
        {
            return new BatchSeedJobConfiguration(canonicalString);
        }

        public virtual void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            var config = (BatchSeedJobConfiguration) configuration;
            var batchId = config.BatchId;
            BatchEntity batch = commandContext.BatchManager.FindBatchById(batchId);
            EnsureUtil.EnsureNotNull("Batch with id '" + batchId + "' cannot be found", "batch", batch);
            
            IBatchJobHandler batchJobHandler =
                commandContext.ProcessEngineConfiguration.BatchHandlers[batch.Type];

            var done = batchJobHandler.CreateJobs(batch);

            if (!done)
            {
                batch.CreateSeedJob();
            }
            else
            {
                // create monitor job initially without due date to
                // enable rapid completion of simple batches
                batch.CreateMonitorJob(false);
            }
        }

        public virtual void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            // do nothing
        }

        public class BatchSeedJobConfiguration : IJobHandlerConfiguration
        {
            protected internal string batchId;

            public BatchSeedJobConfiguration(string batchId)
            {
                this.batchId = batchId;
            }

            public virtual string BatchId
            {
                get { return batchId; }
            }

            public virtual string ToCanonicalString()
            {
                return batchId;
            }
        }
    }
}