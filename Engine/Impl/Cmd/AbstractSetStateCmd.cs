using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     
    /// </summary>
    public abstract class AbstractSetStateCmd : ICommand<object>
    {
        protected internal const string SuspensionStateProperty = "suspensionState";
        protected internal DateTime? ExecutionDate;

        protected internal bool includeSubResources;
        protected internal bool IsLogUserOperationDisabled;

        public AbstractSetStateCmd(bool includeSubResources, DateTime? executionDate)
        {
            this.includeSubResources = includeSubResources;
            this.ExecutionDate = executionDate;
        }

        protected internal virtual bool LogUserOperationDisabled
        {
            get { return IsLogUserOperationDisabled; }
        }

        protected internal virtual bool IncludeSubResources
        {
            get { return includeSubResources; }
        }

        protected internal virtual string DelayedExecutionJobHandlerType
        {
            get { return null; }
        }

        protected internal virtual IJobHandlerConfiguration JobHandlerConfiguration => null;

        protected internal virtual AbstractSetStateCmd NextCommand => null;

        protected internal abstract string LogEntryOperation { get; }
        
        public virtual object Execute(CommandContext commandContext)
        {
            CheckParameters(commandContext);
            CheckAuthorization(commandContext);

            if (ExecutionDate == null)
            {
                UpdateSuspensionState(commandContext, NewSuspensionState);

                if (IncludeSubResources)
                {
                    var cmd = NextCommand;
                    if (cmd != null)
                    {
                        cmd.DisableLogUserOperation();
                        commandContext.RunWithoutAuthorization(() =>
                        {
                            cmd.Execute(commandContext);
                        });
                    }
                }

                TriggerHistoryEvent(commandContext);
            }
            else
            {
                ScheduleSuspensionStateUpdate(commandContext);
            }

            if (!LogUserOperationDisabled)
                LogUserOperation(commandContext);

            return null;
        }

        protected internal virtual void TriggerHistoryEvent(CommandContext commandContext)
        {
        }

        public virtual void DisableLogUserOperation()
        {
            IsLogUserOperationDisabled = true;
        }

        protected internal virtual void ScheduleSuspensionStateUpdate(CommandContext commandContext)
        {
            TimerEntity timer = new TimerEntity();

            var jobHandlerConfiguration = JobHandlerConfiguration;

            timer.Duedate = (DateTime)ExecutionDate;
            timer.JobHandlerType = DelayedExecutionJobHandlerType;
            timer.JobHandlerConfigurationRaw = jobHandlerConfiguration.ToCanonicalString();

            commandContext.JobManager.Schedule(timer);
        }

        protected internal abstract void CheckAuthorization(CommandContext commandContext);

        protected internal abstract void CheckParameters(CommandContext commandContext);

        protected internal abstract void UpdateSuspensionState(CommandContext commandContext, ISuspensionState suspensionState);

        protected internal abstract void LogUserOperation(CommandContext commandContext);
        

        protected internal abstract ISuspensionState NewSuspensionState {get;}
    }
}