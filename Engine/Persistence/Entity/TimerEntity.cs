using System;
using System.Transactions;
using ESS.FW.Bpm.Engine.Impl.Calendar;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;



namespace ESS.FW.Bpm.Engine.Persistence.Entity
{

    /// <summary>
	/// </summary>
	public class TimerEntity : JobEntity
    {

        public const string TYPE = "timer";

        protected internal string repeat;


        public TimerEntity()
        {
        }

        public TimerEntity(TimerDeclarationImpl timerDeclaration)
        {
            repeat = timerDeclaration.Repeat;
        }

        protected internal TimerEntity(TimerEntity te)
        {
            JobHandlerConfigurationRaw = te.JobHandlerConfigurationRaw;
            JobHandlerType = te.JobHandlerType;
            Exclusive = te.Exclusive;
            repeat = te.repeat;
            retries = te.retries;
            ExecutionId = te.ExecutionId;
            ProcessInstanceId = te.ProcessInstanceId;
            JobDefinitionId = te.JobDefinitionId;
            SuspensionState = te.SuspensionState;
            DeploymentId = te.DeploymentId;
            ProcessDefinitionId = te.ProcessDefinitionId;
            ProcessDefinitionKey = te.ProcessDefinitionKey;
            TenantId = te.TenantId;
        }

        protected internal override void PreExecute(CommandContext commandContext)
        {
            if (JobHandler is TimerEventJobHandler)
            {
                TimerEventJobHandler.TimerJobConfiguration configuration = (TimerEventJobHandler.TimerJobConfiguration)JobHandlerConfiguration;
                if (repeat != null && !configuration.FollowUpJobCreated)
                {
                    // this timer is a repeating timer and
                    // a follow up timer job has not been scheduled yet

                    DateTime? newDueDate = CalculateRepeat();

                    if (newDueDate != null)
                    {
                        // the listener is added to the transaction as SYNC on ROLLABCK,
                        // when it is necessary to schedule a new timer job invocation.
                        // If the transaction does not rollback, it is ignored.
                        ProcessEngineConfigurationImpl processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
                        ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequiresNew;
                        RepeatingFailedJobListener listener = CreateRepeatingFailedJobListener(commandExecutor);

                        commandContext.TransactionContext.AddTransactionListener(TransactionJavaStatus.RolledBack, listener);

                        // create a new timer job
                        CreateNewTimerJob(newDueDate);
                    }
                }
            }
        }

        protected internal virtual RepeatingFailedJobListener CreateRepeatingFailedJobListener(ICommandExecutor commandExecutor)
        {
            return new RepeatingFailedJobListener(commandExecutor, Id);
        }

        public virtual void CreateNewTimerJob(DateTime? dueDate)
        {
            // create new timer job
            TimerEntity newTimer = new TimerEntity(this);
            newTimer.Duedate = dueDate;
            context.Impl.Context.CommandContext.JobManager.Schedule(newTimer);
        }

        public virtual DateTime? CalculateRepeat()
        {
            IBusinessCalendar businessCalendar = context.Impl.Context.ProcessEngineConfiguration.BusinessCalendarManager.GetBusinessCalendar(CycleBusinessCalendar.Name);
            return businessCalendar.ResolveDuedate(repeat);
        }


        public override string Repeat
        {
            get
            {
                return repeat;
            }
            set
            {
                this.repeat = value;
            }
        }

        public override string Type => TYPE;

        //public override string ToString()
        //{
        //    return this.GetType().Name + "[repeat=" + repeat + ", id=" + id + ", revision=" + revision + ", duedate=" + duedate + ", lockOwner=" + lockOwner + ", lockExpirationTime=" + lockExpirationTime + ", executionId=" + executionId + ", processInstanceId=" + processInstanceId + ", isExclusive=" + IsExclusive + ", retries=" + retries + ", jobHandlerType=" + jobHandlerType + ", jobHandlerConfiguration=" + jobHandlerConfiguration + ", exceptionByteArray=" + exceptionByteArray + ", exceptionByteArrayId=" + exceptionByteArrayId + ", exceptionMessage=" + exceptionMessage + ", deploymentId=" + deploymentId + "]";
        //}

    }

}