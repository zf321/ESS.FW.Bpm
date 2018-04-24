using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Batch.Impl
{

    /// <summary>
    ///     Job handler for batch monitor jobs. The batch monitor job
    ///     polls for the completion of the batch.
    /// </summary>
    public class BatchMonitorJobHandler : IJobHandler<BatchMonitorJobHandler.BatchMonitorJobConfiguration>
    {
        public const string TYPE = "batch-monitor-job";

        public virtual string Type
        {
            get { return TYPE; }
        }

        public virtual void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            var conf = (BatchMonitorJobConfiguration) configuration;
            var batchId = conf.BatchId;
            BatchEntity batch = commandContext.BatchManager.FindBatchById(conf.BatchId);
            EnsureUtil.EnsureNotNull("Batch with id '" + batchId + "' cannot be found", "batch", batch);

            bool completed = batch.Completed;

            if (!completed)
            {
                //batch.CreateMonitorJob(true);
            }
            else
            {
                batch.Delete(false);
            }
        }

        public virtual IJobHandlerConfiguration NewConfiguration(string canonicalString)
        {
            return new BatchMonitorJobConfiguration(canonicalString);
        }

        public virtual void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            // do nothing
        }

        public class BatchMonitorJobConfiguration : IJobHandlerConfiguration
        {
            protected internal string batchId;

            public BatchMonitorJobConfiguration(string batchId)
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