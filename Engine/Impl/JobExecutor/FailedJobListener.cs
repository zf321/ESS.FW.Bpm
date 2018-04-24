using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Impl.Cfg;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    ///      Bernd Ruecker
    /// </summary>
    public class FailedJobListener : ICommand<object>
    {
        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;

        protected internal ICommandExecutor commandExecutor;
        protected internal System.Exception Exception;
        protected internal string JobId;

        private int _countRetries = 0;
        private int _totalRetries = ProcessEngineConfigurationImpl.DefaultFailedJobListenerMaxRetries;

        public FailedJobListener(ICommandExecutor commandExecutor, string jobId, System.Exception exception)
        {
            this.commandExecutor = commandExecutor;
            this.JobId = jobId;
            this.Exception = exception;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            InitTotalRetries(commandContext);

            LogJobFailure(commandContext);

            var failedJobCommandFactory = commandContext.FailedJobCommandFactory;
            var cmd = failedJobCommandFactory.GetCommand(JobId, Exception);

            return commandExecutor.Execute(new CommandAnonymousInnerClass(this, commandContext, cmd));
        }

        protected internal virtual void FireHistoricJobFailedEvt(JobEntity job)
        {
            var commandContext = Context.CommandContext;

            // the given job failed and a rollback happened,
            // that's why we have to increment the job
            // sequence counter once again
            job.IncrementSequenceCounter();

            commandContext.HistoricJobLogManager.FireJobFailedEvent(job, Exception);
        }

        protected internal virtual void LogJobFailure(CommandContext commandContext)
        {
            if (commandContext.ProcessEngineConfiguration.MetricsEnabled)
                commandContext.ProcessEngineConfiguration.MetricsRegistry.MarkOccurrence(Engine.Management.Metrics.JobFailed);
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly ICommand<object> _cmd;
            private readonly FailedJobListener _outerInstance;

            private CommandContext _commandContext;

            public CommandAnonymousInnerClass(FailedJobListener outerInstance, CommandContext commandContext,
                ICommand<object> cmd)
            {
                this._outerInstance = outerInstance;
                this._commandContext = commandContext;
                this._cmd = cmd;
            }


            public virtual object Execute(CommandContext commandContext)
            {
                JobEntity job = commandContext.JobManager.FindJobById(_outerInstance.JobId);

                if (job != null)
                {
                    _outerInstance.FireHistoricJobFailedEvt(job);
                    _cmd.Execute(commandContext);
                }
                else
                {
                    Log.DebugFailedJobNotFound(_outerInstance.JobId);
                }
                return null;
            }
        }

        public void IncrementCountRetries()
        {
            this._countRetries++;
        }

        public int GetRetriesLeft()
        {
            return Math.Max(0, _totalRetries - _countRetries);
        }

        private void InitTotalRetries(CommandContext commandContext)
        {
            _totalRetries = commandContext.ProcessEngineConfiguration.FailedJobListenerMaxRetries;
        }
    }
}