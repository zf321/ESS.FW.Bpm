using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace Engine.Tests.Bpmn.ExecutionListener
{
    [Serializable]
    public class RecorderExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {

        private const long SerialVersionUid = 1L;

        private FixedValue _parameter;

        private static IList<RecorderExecutionListener.RecordedEvent> _recordedEvents = new List<RecorderExecutionListener.RecordedEvent>();

        public class RecordedEvent
        {
            internal readonly string activityId;
            internal readonly string eventName;
            internal readonly string activityName;
            internal readonly string parameter;
            internal readonly string activityInstanceId;
            internal readonly string transitionId;
            internal readonly bool canceled;
            internal readonly string executionId;

            public RecordedEvent(string activityId, string activityName, string eventName, string parameter, string activityInstanceId, string transitionId, bool canceled, string executionId)
            {
                this.activityId = activityId;
                this.activityName = activityName;
                this.parameter = parameter;
                this.eventName = eventName;
                this.activityInstanceId = activityInstanceId;
                this.transitionId = transitionId;
                this.canceled = canceled;
                this.executionId = executionId;
            }

            public virtual string ActivityId
            {
                get
                {
                    return activityId;
                }
            }

            public virtual string EventName
            {
                get
                {
                    return eventName;
                }
            }


            public virtual string ActivityName
            {
                get
                {
                    return activityName;
                }
            }


            public virtual string Parameter
            {
                get
                {
                    return parameter;
                }
            }

            public virtual string ActivityInstanceId
            {
                get
                {
                    return activityInstanceId;
                }
            }

            public virtual string TransitionId
            {
                get
                {
                    return transitionId;
                }
            }

            public virtual bool Canceled
            {
                get
                {
                    return canceled;
                }
            }

            public virtual string ExecutionId
            {
                get
                {
                    return executionId;
                }
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void notify(delegate.IDelegateExecution execution) throws Exception
        //public virtual void notify(IDelegateExecution execution)
        //{
        //    ExecutionEntity executionCasted = ((ExecutionEntity)execution);
        //    string parameterValue = null;
        //    if (parameter != null)
        //    {
        //        parameterValue = (string)parameter.GetValue(execution);
        //    }

        //    string activityName = null;
        //    if (executionCasted.Activity != null)
        //    {
        //        activityName = executionCasted.Activity.Properties.Get(new PropertyKey<string>("name"));
        //    }

        //    recordedEvents.Add(new RecordedEvent(executionCasted.ActivityId, activityName, execution.EventName, parameterValue, execution.ActivityInstanceId, execution.CurrentTransitionId, execution.Canceled, execution.Id));
        //}

        public void Notify(IBaseDelegateExecution execution)
        {
            ExecutionEntity executionCasted = ((ExecutionEntity)execution);
            string parameterValue = null;
            if (_parameter != null)
            {
                parameterValue = (string)_parameter.GetValue(execution);
            }

            string activityName = null;
            if (executionCasted.Activity != null)
            {
                activityName = executionCasted.Activity.Properties.Get(new PropertyKey<string>("name"));
            }

            _recordedEvents.Add(new RecordedEvent(executionCasted.ActivityId, activityName, execution.EventName, parameterValue, (execution as IDelegateExecution).ActivityInstanceId, (execution as IDelegateExecution).CurrentTransitionId, (execution as IDelegateExecution).Canceled, execution.Id));
        }


        public static void Clear()
        {
            _recordedEvents.Clear();
        }


        public static IList<RecordedEvent> RecordedEvents
        {
            get
            {
                return _recordedEvents;
            }
        }


    }

}