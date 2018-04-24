using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Compensate
{
    [Serializable]
    public class ReadLocalVariableListener : IDelegateListener<IBaseDelegateExecution>
    {

        private const long serialVersionUID = 1L;

        protected internal IList<VariableEvent> variableEvents = new List<VariableEvent>();
        protected internal string VariableName;

        public ReadLocalVariableListener(string VariableName)
        {
            this.VariableName = VariableName;
        }

        public virtual IList<VariableEvent> VariableEvents
        {
            get
            {
                return variableEvents;
            }
            set
            {
                this.VariableEvents = value;
            }
        }

        public void Notify(IBaseDelegateExecution execution)
        {
            if (!execution.HasVariableLocal(VariableName))
            {
                return;
            }

            object value = execution.GetVariableLocal(VariableName);

            VariableEvent @event = new VariableEvent();
            @event.VariableName = VariableName;
            @event.VariableValue = value;
            @event.EventName = execution.EventName;
            @event.ActivityInstanceId = (execution as IDelegateExecution)?.ActivityInstanceId;

            variableEvents.Add(@event);
        }


        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void Notify(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public override void Notify(IDelegateExecution execution)
        //{
        //    if (!execution.HasVariableLocal(VariableName))
        //    {
        //        return;
        //    }

        //    object value = execution.GetVariableLocal(VariableName);

        //    VariableEvent @event = new VariableEvent();
        //    @event.VariableName = VariableName;
        //    @event.VariableValue = value;
        //    @event.EventName = execution.EventName;
        //    @event.ActivityInstanceId = execution.ActivityInstanceId;

        //    variableEvents.Add(@event);
        //}

        [Serializable]
        public class VariableEvent
        {

            internal const long serialVersionUID = 1L;

            protected internal string variableName;
            protected internal object variableValue;

            protected internal string activityInstanceId;
            protected internal string eventName;

            public virtual string VariableName
            {
                get { return variableName; }
                set { variableName = value; }
            }

            public virtual object VariableValue
            {
                get { return variableValue; }
                set { variableValue = value; }
            }

            public virtual string EventName
            {
                get { return eventName; }
                set { eventName = value; }
            }

            public virtual string ActivityInstanceId
            {
                get { return activityInstanceId; }
                set { activityInstanceId = value; }
            }
        }
    }

}