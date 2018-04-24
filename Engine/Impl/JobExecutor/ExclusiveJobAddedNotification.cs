using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public class ExclusiveJobAddedNotification : ITransactionListener
    {
        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;
        protected internal readonly JobExecutorContext JobExecutorContext;

        protected internal readonly string JobId;

        public ExclusiveJobAddedNotification(string jobId, JobExecutorContext jobExecutorContext)
        {
            this.JobId = jobId;
            this.JobExecutorContext = jobExecutorContext;
        }

        public virtual void Execute(CommandContext commandContext)
        {
            Log.DebugAddingNewExclusiveJobToJobExecutorCOntext(JobId);
            JobExecutorContext.CurrentProcessorJobQueue.Enqueue(JobId);
            LogExclusiveJobAdded(commandContext);
        }

        protected internal virtual void LogExclusiveJobAdded(CommandContext commandContext)
        {
            if (commandContext.ProcessEngineConfiguration.MetricsEnabled)
                commandContext.ProcessEngineConfiguration.MetricsRegistry.MarkOccurrence(Engine.Management.Metrics.JobLockedExclusive);
        }
    }
}