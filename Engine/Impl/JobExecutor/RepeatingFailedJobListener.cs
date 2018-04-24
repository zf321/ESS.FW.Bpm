using System;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public class RepeatingFailedJobListener : ITransactionListener
    {
        protected internal ICommandExecutor CommandExecutor;
        protected internal string JobId;

        public RepeatingFailedJobListener(ICommandExecutor commandExecutor, string jobId)
        {
            this.CommandExecutor = commandExecutor;
            this.JobId = jobId;
        }

        public virtual void Execute(CommandContext commandContext)
        {
            var cmd = new CreateNewTimerJobCommand(this, JobId);
            CommandExecutor.Execute(cmd);
        }

        protected internal class CreateNewTimerJobCommand : ICommand<object>
        {
            private readonly RepeatingFailedJobListener _outerInstance;


            protected internal string JobId;

            public CreateNewTimerJobCommand(RepeatingFailedJobListener outerInstance, string jobId)
            {
                this._outerInstance = outerInstance;
                this.JobId = jobId;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                TimerEntity failedJob = (TimerEntity)commandContext.JobManager.FindJobById(JobId);

                DateTime? newDueDate = failedJob.CalculateRepeat();

                if (newDueDate != null)
                {
                    failedJob.CreateNewTimerJob(newDueDate);

                    // update configuration of failed job
                    var config = (TimerEventJobHandler.TimerJobConfiguration)failedJob.JobHandlerConfiguration;
                    config.FollowUpJobCreated = true;
                    failedJob.JobHandlerConfiguration = config;
                }

                return null;
            }
        }
    }
}