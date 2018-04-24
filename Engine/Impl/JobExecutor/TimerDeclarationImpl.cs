using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Common.Utilities;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class TimerDeclarationImpl : JobDeclaration<TimerEntity>
    {
        private const long SerialVersionUid = 1L;

        protected internal Engine.Delegate.IExpression Description;
        protected internal string eventScopeActivityId;
        protected internal bool IsInterruptingTimer; // For boundary timers
        protected internal bool? IsParallelMultiInstance;

        protected internal string rawJobHandlerConfiguration;

        protected internal TimerDeclarationType Type;

        public TimerDeclarationImpl(Engine.Delegate.IExpression expression, TimerDeclarationType type, string jobHandlerType)
            : base(jobHandlerType)
        {
            Description = expression;
            this.Type = type;
        }

        public virtual bool InterruptingTimer
        {
            get { return IsInterruptingTimer; }
            set { IsInterruptingTimer = value; }
        }


        public virtual string Repeat { get; }

        public virtual string EventScopeActivityId
        {
            set { eventScopeActivityId = value; }
            get { return eventScopeActivityId; }
        }

        public virtual string RawJobHandlerConfiguration
        {
            set { rawJobHandlerConfiguration = value; }
        }


        protected internal override TimerEntity NewJobInstance(object execution)
        {
            TimerEntity timer = new TimerEntity(this);
            if (execution != null)
            {
                timer.Execution = execution as ExecutionEntity;
            }

            return timer;
        }

        public virtual void UpdateJob(TimerEntity timer)
        {
            InitializeConfiguration(timer.Execution, timer);
        }

        protected internal virtual void InitializeConfiguration(ExecutionEntity context, TimerEntity job)
        {
            var businessCalendar =
                Context.ProcessEngineConfiguration.BusinessCalendarManager.GetBusinessCalendar(Type.GetDescription());

            if (Description == null)
            {
                throw new ProcessEngineException("Timer '" + context.ActivityId +
                                                 "' was not configured with a valid duration/time");
            }

            string dueDateString = null;
            //throw new NotImplementedException();
            DateTime? duedate=null ;

            // ACT-1415: timer-declaration on start-event may contain expressions NOT
            // evaluating variables but other context, evaluating should happen nevertheless
            IVariableScope scopeForExpression = context;
            if (scopeForExpression == null)
            {
                scopeForExpression = StartProcessVariableScope.SharedInstance;
            }

            var dueDateValue = Description.GetValue(scopeForExpression);
            if (dueDateValue is string)
            {
                dueDateString = (string)dueDateValue;
            }
            else if (dueDateValue is DateTime)
            {
                duedate = (DateTime)dueDateValue;
            }
            else
            {
                throw new ProcessEngineException("Timer '" + context.ActivityId +
                                                 "' was not configured with a valid duration/time, either hand in a java.Util.Date or a String in format 'yyyy-MM-dd'T'hh:mm:ss'");
            }
            //throw new NotImplementedException();
            if (duedate == null)
            {
                duedate = businessCalendar.ResolveDuedate(dueDateString);
            }

            job.Duedate = (DateTime)duedate;

            if (Type == TimerDeclarationType.Cycle&&
                !ReferenceEquals(jobHandlerType, TimerCatchIntermediateEventJobHandler.TYPE))
            {
                // See ACT-1427: A boundary timer with a cancelActivity='true', doesn't need to repeat itself
                if (!IsInterruptingTimer)
                {
                    var prepared = PrepareRepeat(dueDateString);
                    job.Repeat = prepared;
                }
            }
        }
        protected override void PostInitialize(object context, TimerEntity job)
        {
            PostInitialize((ExecutionEntity)context, job);
        }
        protected internal  void PostInitialize(ExecutionEntity execution, TimerEntity timer)
        {
            InitializeConfiguration(execution, timer);
        }

        protected internal virtual string PrepareRepeat(string dueDate)
        {
            if (dueDate.StartsWith("R", StringComparison.Ordinal) && (dueDate.Split("/", true).Length == 2))
            {
                var pattern = "yyyy-MM-dd'T'HH:mm:ss";
                return ClockUtil.CurrentTime.ToString(pattern);
            }
            return dueDate;
        }

        public virtual TimerEntity CreateTimerInstance(ExecutionEntity execution)
        {
            return CreateTimer(execution);
        }

        public virtual TimerEntity CreateStartTimerInstance(string deploymentId)
        {
            return CreateTimer(deploymentId);
        }

        public virtual TimerEntity CreateTimer(string deploymentId)
        {
            TimerEntity timer = CreateJobInstance(null);
            timer.DeploymentId = deploymentId;
            ScheduleTimer(timer);
            return timer;
        }

        public virtual TimerEntity CreateTimer(ExecutionEntity execution)
        {
            TimerEntity timer = CreateJobInstance(execution);
            ScheduleTimer(timer);
            return timer;
        }

        protected internal virtual void ScheduleTimer(TimerEntity timer)
        {
            Context.CommandContext.JobManager.Schedule(timer);
        }

        protected internal override ExecutionEntity ResolveExecution(object context)
        {
            return context as ExecutionEntity;
        }

        protected internal override IJobHandlerConfiguration ResolveJobHandlerConfiguration(object context)
        {
            return ResolveJobHandler().NewConfiguration(rawJobHandlerConfiguration);
        }

        public static IDictionary<string, TimerDeclarationImpl> GetDeclarationsForScope(IPvmScope scope)
        {
            if (scope == null)
                return null;

            var result = scope.Properties.Get(BpmnProperties.TimerDeclarations);
            if (result != null)
                return result;
            return null;
        }
    }
}