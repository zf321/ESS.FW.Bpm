using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using IExpression = ESS.FW.Bpm.Engine.Delegate.IExpression;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    [Serializable]
    public class EventSubscriptionDeclaration
    {
        private const long SerialVersionUid = 1L;
        protected internal readonly IExpression EventName;

        protected internal readonly EventType eventType;
        protected internal string activityId;

        protected internal bool async;
        protected internal string eventScopeActivityId;
        protected internal bool IsStartEvent;

        protected internal EventSubscriptionJobDeclaration jobDeclaration;

        public EventSubscriptionDeclaration(IExpression eventExpression, EventType eventType)
        {
            EventName = eventExpression;
            this.eventType = eventType;
        }

        /// <summary>
        ///     Returns the name of the event without evaluating the possible expression that it might contain.
        /// </summary>
        public virtual string UnresolvedEventName
        {
            get { return EventName.ExpressionText; }
        }

        public virtual bool EventNameLiteralText
        {
            get { return EventName.LiteralText; }
        }

        public virtual bool Async
        {
            get { return async; }
            set { async = value; }
        }


        public virtual string ActivityId
        {
            get { return activityId; }
            set { activityId = value; }
        }


        public virtual string EventScopeActivityId
        {
            get { return eventScopeActivityId; }
            set { eventScopeActivityId = value; }
        }


        public virtual bool StartEvent
        {
            get { return IsStartEvent; }
            set { IsStartEvent = value; }
        }


        public virtual string EventType
        {
            get { return eventType.Name; }
        }

        public virtual EventSubscriptionJobDeclaration JobDeclaration
        {
            set { jobDeclaration = value; }
        }

        protected internal virtual bool ExpressionAvailable
        {
            get { return EventName != null; }
        }

        public static IDictionary<string, EventSubscriptionDeclaration> GetDeclarationsForScope(IPvmScope scope)
        {
            if (scope == null)
                return null;

            return scope.Properties.Get(BpmnProperties.EventSubscriptionDeclarations);
        }

        public virtual bool HasEventName()
        {
            return EventName != null && !string.IsNullOrWhiteSpace(UnresolvedEventName);
            //!((EventName == null) ||
            //"".Equals(UnresolvedEventName.Trim(), StringComparison.CurrentCultureIgnoreCase));

        }

        public virtual EventSubscriptionEntity CreateSubscriptionForStartEvent(ProcessDefinitionEntity processDefinition)
        {
            var eventSubscriptionEntity = new EventSubscriptionEntity(eventType);

            IVariableScope scopeForExpression = StartProcessVariableScope.SharedInstance;
            string eventName = ResolveExpressionOfEventName(scopeForExpression);
            eventSubscriptionEntity.EventName = eventName;
            eventSubscriptionEntity.ActivityId = activityId;
            eventSubscriptionEntity.Configuration = processDefinition.Id;
            eventSubscriptionEntity.TenantId = processDefinition.TenantId;

            return eventSubscriptionEntity;
            //return null;
        }

        /// <summary>
        ///     Creates and inserts a subscription entity depending on the message type of this declaration.
        /// </summary>
        public virtual EventSubscriptionEntity CreateSubscriptionForExecution(ExecutionEntity execution)
        {
            var eventSubscriptionEntity = new EventSubscriptionEntity(execution, eventType);

            eventSubscriptionEntity.EventName = ResolveExpressionOfEventName(execution);
            if (activityId != null)
            {
                ActivityImpl activity = execution.GetProcessDefinition().FindActivity(activityId) as ActivityImpl;
                eventSubscriptionEntity.Activity = activity;
            }

            eventSubscriptionEntity.Insert();
            LegacyBehavior.RemoveLegacySubscriptionOnParent(execution, eventSubscriptionEntity);

            return eventSubscriptionEntity;
            //return null;
        }

        /// <summary>
        ///     Resolves the event name within the given scope.
        /// </summary>
        public virtual string ResolveExpressionOfEventName(IVariableScope scope)
        {
            if (ExpressionAvailable)
            {
                //TODO 表达式替换
                return EventName.ExpressionText;
                return (string)EventName.GetValue(scope);
            }
            return null;
        }

        public virtual void UpdateSubscription(EventSubscriptionEntity eventSubscription)
        {
            string eventName = ResolveExpressionOfEventName(eventSubscription.Execution);
            eventSubscription.EventName = eventName;
            eventSubscription.ActivityId = activityId;
        }
    }
}