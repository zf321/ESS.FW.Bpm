using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Unlock job.
    ///     
    /// </summary>
    public class UnlockJobCmd : ICommand<object>
    {
        protected internal const long SerialVersionUid = 1L;

        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;

        private readonly string _jobId;

        public UnlockJobCmd(string jobId)
        {
            _jobId = jobId;
        }

        protected internal virtual JobEntity Job => context.Impl.Context.CommandContext.JobManager.FindJobById(_jobId);

        public virtual object Execute(CommandContext commandContext)
        {
            var job = Job;

            if (context.Impl.Context.JobExecutorContext == null)
                EnsureUtil.EnsureNotNull("Job with id " + _jobId + " does not exist", "job", job);
            else if (context.Impl.Context.JobExecutorContext != null && job == null)
                Log.DebugAcquiredJobNotFound(_jobId);

            job?.Unlock();
            return null;
        }
    }
}