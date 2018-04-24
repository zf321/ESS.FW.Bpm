using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class DecrementJobRetriesCmd : JobRetryCmd
    {
        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;

        public DecrementJobRetriesCmd(string jobId, System.Exception exception) : base(jobId, exception)
        {
        }

        public override object Execute(CommandContext commandContext)
        {
            var job = Job;
            if (job != null)
            {
                job.Unlock();
                LogException(job);
                DecrementRetries(job);
                NotifyAcquisition(commandContext);
            }
            else
            {
                Log.DebugFailedJobNotFound(JobId);
            }
            return null;
        }
    }
}