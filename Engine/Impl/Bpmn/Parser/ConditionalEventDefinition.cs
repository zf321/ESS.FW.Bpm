using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Event;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     Represents the conditional event definition corresponding to the
    ///     ConditionalEvent defined by the BPMN 2.0 spec.
    ///      
    /// </summary>
    [Serializable]
    public class ConditionalEventDefinition : EventSubscriptionDeclaration
    {
        private const long SerialVersionUid = 1L;

        protected internal readonly ICondition Condition;
        protected internal ActivityImpl conditionalActivity;
        protected internal bool interrupting;
        protected internal ISet<string> variableEvents;
        protected internal string variableName;

        public ConditionalEventDefinition(ICondition condition, ActivityImpl conditionalActivity)
            : base(null, Event.EventType.Conditonal)
        {
            activityId = conditionalActivity.ActivityId;
            this.conditionalActivity = conditionalActivity;
            this.Condition = condition;
        }

        public virtual ActivityImpl ConditionalActivity
        {
            get { return conditionalActivity; }
            set { conditionalActivity = value; }
        }


        public virtual bool Interrupting
        {
            get { return interrupting; }
            set { interrupting = value; }
        }


        public virtual string VariableName
        {
            get { return variableName; }
            set { variableName = value; }
        }


        public virtual ISet<string> VariableEvents
        {
            get { return variableEvents; }
            set { variableEvents = value; }
        }


        public virtual bool ShouldEvaluateForVariableEvent(VariableEvent @event)
        {
            return (ReferenceEquals(variableName, null) || @event.VariableInstance.Name.Equals(variableName)) &&
                   ((variableEvents == null) || (variableEvents.Count == 0) || variableEvents.Contains(@event.EventName));
        }

        public virtual bool Evaluate(IDelegateExecution execution)
        {
            if (Condition != null)
                return Condition.Evaluate(execution, execution);
            throw new InvalidOperationException("Condtional event must have a condition!");
        }

        public virtual bool TryEvaluate(IDelegateExecution execution)
        {
            if (Condition != null)
                return Condition.TryEvaluate(execution, execution);
            throw new InvalidOperationException("Condtional event must have a condition!");
        }

        public virtual bool TryEvaluate(VariableEvent variableEvent, IDelegateExecution execution)
        {
            return ((variableEvent == null) || ShouldEvaluateForVariableEvent(variableEvent)) && TryEvaluate(execution);
        }
    }
}