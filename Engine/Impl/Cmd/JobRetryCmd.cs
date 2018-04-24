using System.Transactions;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class JobRetryCmd : ICommand<object>
    {
        protected internal const long SerialVersionUid = 1L;
        protected internal System.Exception Exception;
        protected internal string JobId;

        public JobRetryCmd(string jobId, System.Exception exception)
        {
            this.JobId = jobId;
            this.Exception = exception;
        }

        protected internal virtual JobEntity Job => context.Impl.Context.CommandContext.JobManager.FindJobById(JobId);

        protected internal virtual void LogException(JobEntity job)
        {
            if (Exception != null)
            {
                job.ExceptionMessage = Exception.Message;
                job.ExceptionStacktrace = ExceptionStacktrace;
            }
        }

        protected internal virtual void DecrementRetries(JobEntity job)
        {
            if ((Exception == null) || ShouldDecrementRetriesFor(Exception))
            {
                job.Retries = job.Retries - 1;
            }
        }

        protected internal virtual string ExceptionStacktrace => ExceptionUtil.GetExceptionStacktrace(Exception);

        protected internal virtual bool ShouldDecrementRetriesFor(System.Exception t)
        {
            return !(t is OptimisticLockingException);
        }       

        protected internal virtual void NotifyAcquisition(CommandContext commandContext)
        {
            var jobExecutor = context.Impl.Context.ProcessEngineConfiguration.JobExecutor;
            var messageAddedNotification = new MessageAddedNotification(jobExecutor);
            var transactionContext = commandContext.TransactionContext;
            transactionContext.AddTransactionListener(TransactionJavaStatus.Committed, messageAddedNotification);
        }

        public abstract object Execute(CommandContext commandContext);
    }
}