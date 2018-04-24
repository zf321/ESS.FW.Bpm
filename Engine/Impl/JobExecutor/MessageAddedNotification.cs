using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///      
    /// </summary>
    public class MessageAddedNotification : ITransactionListener
    {
        private readonly JobExecutorLogger _log = ProcessEngineLogger.JobExecutorLogger;

        protected internal JobExecutor JobExecutor;

        public MessageAddedNotification(JobExecutor jobExecutor)
        {
            this.JobExecutor = jobExecutor;
        }

        public virtual void Execute(CommandContext commandContext)
        {
            _log.DebugNotifyingJobExecutor("notifying job executor of new job");
            JobExecutor.JobWasAdded();
        }
    }
}