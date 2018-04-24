using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.tree;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using System.Linq.Expressions;
using System.Linq;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Helper
{
    /// <summary>
    ///     
    /// </summary>
    public class CompensationUtil
    {
        /// <summary>
        ///     name of the signal that is thrown when a compensation handler completed
        /// </summary>
        public const string SignalCompensationDone = "compensationDone";

        /// <summary>
        ///     we create a separate execution for each compensation handler invocation.
        /// </summary>
        public static void ThrowCompensationEvent(IList<EventSubscriptionEntity> eventSubscriptions,
            IActivityExecution execution, bool async)
        {
            // first spawn the compensating executions
            foreach (var eventSubscription in eventSubscriptions)
            {
                // check whether compensating execution is already created
                // (which is the case when compensating an embedded subprocess,
                // where the compensating execution is created when leaving the subprocess
                // and holds snapshot data).
                var compensatingExecution = GetCompensatingExecution(eventSubscription);
                if (compensatingExecution != null)
                {

                    if (compensatingExecution.Parent != execution)
                    {
                        // move the compensating execution under this execution if this is not the case yet
                        compensatingExecution.Parent = (PvmExecutionImpl)execution;
                    }

                    compensatingExecution.IsEventScope = false;
                }
                else
                {
                    compensatingExecution = (ExecutionEntity)execution.CreateExecution();
                    eventSubscription.Configuration = compensatingExecution.Id;
                }
                compensatingExecution.IsConcurrent = true;
            }

            // signal compensation events in REVERSE order of their 'created' timestamp
            //eventSubscriptions.Sort(new ComparatorAnonymousInnerClass());
            eventSubscriptions = eventSubscriptions.OrderBy(m => m.Created).ToList();
            foreach (var compensateEventSubscriptionEntity in eventSubscriptions)
            {
                compensateEventSubscriptionEntity.EventReceived(null, async);
            }
        }

        /// <summary>
        ///     creates an event scope for the given execution:
        ///     create a new event scope execution under the parent of the given execution
        ///     and move all event subscriptions to that execution.
        ///     this allows us to "remember" the event subscriptions after finishing a
        ///     scope
        /// </summary>
        public static void CreateEventScopeExecution(ExecutionEntity execution)
        {
            // parent execution is a subprocess or a miBody
            ActivityImpl activity = (ActivityImpl) execution.Activity;
            ExecutionEntity scopeExecution = (ExecutionEntity)execution.FindExecutionForFlowScope(activity.FlowScope);

            IList<EventSubscriptionEntity> eventSubscriptions = execution.CompensateEventSubscriptions;

            if (eventSubscriptions.Count > 0 || HasCompensationEventSubprocess(activity))
            {

                ExecutionEntity eventScopeExecution = scopeExecution.CreateExecution();
                eventScopeExecution.Activity=(execution.Activity);
                eventScopeExecution.ActivityInstanceStarting();
                eventScopeExecution.EnterActivityInstance();
                eventScopeExecution.IsActive = false;
                eventScopeExecution.IsConcurrent = false;
                eventScopeExecution.IsEventScope = true;

                // copy local variables to eventScopeExecution by value. This way,
                // the eventScopeExecution references a 'snapshot' of the local variables
                IDictionary<string, object> variables = execution.VariablesLocal;
                foreach (KeyValuePair<string, object> variable in variables)
                {
                    eventScopeExecution.SetVariableLocal(variable.Key, variable.Value);
                }

                // set event subscriptions to the event scope execution:
                foreach (EventSubscriptionEntity eventSubscriptionEntity in eventSubscriptions)
                {
                    EventSubscriptionEntity newSubscription = EventSubscriptionEntity.CreateAndInsert(eventScopeExecution, EventType.Compensate, eventSubscriptionEntity.Activity);
                    newSubscription.Configuration = eventSubscriptionEntity.Configuration;
                    // use the original date
                    newSubscription.Created = eventSubscriptionEntity.Created;
                }

                // set existing event scope executions as children of new event scope execution
                // (ensuring they don't get removed when 'execution' gets removed)
                foreach (var childEventScopeExecution in execution.EventScopeExecutions)
                {
                    childEventScopeExecution.Parent = eventScopeExecution;
                }

                ActivityImpl compensationHandler = getEventScopeCompensationHandler(execution);
                EventSubscriptionEntity eventSubscription = EventSubscriptionEntity.CreateAndInsert(scopeExecution, EventType.Compensate, compensationHandler);
                eventSubscription.Configuration = eventScopeExecution.Id;

            }
        }

        protected internal static bool HasCompensationEventSubprocess(ActivityImpl activity)
        {
            var compensationHandler = activity.findCompensationHandler();

            return (compensationHandler != null) && compensationHandler.SubProcessScope &&
                   compensationHandler.TriggeredByEvent;
        }

        /// <summary>
        ///     In the context when an event scope execution is created (i.e. a scope such as a subprocess has completed),
        ///     this method returns the compensation handler activity that is going to be executed when by the event scope
        ///     execution.
        ///     This method is not relevant when the scope has a boundary compensation handler.
        /// </summary>
        protected internal static ActivityImpl getEventScopeCompensationHandler(ExecutionEntity execution)
        {
            ActivityImpl activity = (ActivityImpl) execution.Activity;

            ActivityImpl compensationHandler = activity.findCompensationHandler();
            if (compensationHandler != null && compensationHandler.SubProcessScope)
            {
                // subprocess with inner compensation event subprocess
                return compensationHandler;
            }
            else
            {
                // subprocess without compensation handler or
                // multi instance activity
                return activity;
            }
        }

        /// <summary>
        ///     Collect all compensate event subscriptions for scope of given execution.
        /// </summary>
        public static IList<EventSubscriptionEntity> CollectCompensateEventSubscriptionsForScope(
            IActivityExecution execution)
        {
            var scopeExecutionMapping = execution.CreateActivityExecutionMapping();
            var activity = (ScopeImpl) execution.Activity;

            // <LEGACY>: different flow scopes may have the same scope execution =>
            // collect subscriptions in a set
            ISet<EventSubscriptionEntity> subscriptions = new HashSet<EventSubscriptionEntity>();
            ITreeVisitor<ScopeImpl> eventSubscriptionCollector = new TreeVisitorAnonymousInnerClass(execution,
                scopeExecutionMapping, subscriptions);

            new FlowScopeWalker(activity).AddPostVisitor(eventSubscriptionCollector)
                .WalkUntil(element => {
                    bool? flag = (bool?)element.GetProperty(BpmnParse.PropertynameConsumesCompensation);
                    return flag == null || flag == true;
                });

            return new List<EventSubscriptionEntity>(subscriptions);
        }

        /// <summary>
        ///     Collect all compensate event subscriptions for activity on the scope of
        ///     given execution.
        /// </summary>
        public static IList<EventSubscriptionEntity> CollectCompensateEventSubscriptionsForActivity(
            IActivityExecution execution, string activityRef)
        {
            var eventSubscriptions = CollectCompensateEventSubscriptionsForScope(execution);
            var subscriptionActivityId = GetSubscriptionActivityId(execution, activityRef);

            IList<EventSubscriptionEntity> eventSubscriptionsForActivity = new List<EventSubscriptionEntity>();
            foreach (var subscription in eventSubscriptions)
                if (subscriptionActivityId.Equals(subscription.ActivityId))
                    eventSubscriptionsForActivity.Add(subscription);
            return eventSubscriptionsForActivity;
        }

        public static ExecutionEntity GetCompensatingExecution(EventSubscriptionEntity eventSubscription)
        {
            var configuration = eventSubscription.Configuration;
            if (!ReferenceEquals(configuration, null))
            {
                return Context.CommandContext.ExecutionManager.FindExecutionById(configuration);
            }
            return null;
        }

        private static string GetSubscriptionActivityId(IActivityExecution execution, string activityRef)
        {
            ActivityImpl activityToCompensate = (ActivityImpl) ((ExecutionEntity)execution).GetProcessDefinition().FindActivity(activityRef);

            if (activityToCompensate.MultiInstance)
            {

                ActivityImpl flowScope = (ActivityImpl)activityToCompensate.FlowScope;
                return flowScope.ActivityId;
            }
            else
            {

                ActivityImpl compensationHandler = activityToCompensate.findCompensationHandler();
                if (compensationHandler != null)
                {
                    return compensationHandler.ActivityId;
                }
                else
                {
                    // if activityRef = subprocess and subprocess has no compensation handler
                    return activityRef;
                }
            }
        }

        private class ComparatorAnonymousInnerClass : IComparer<EventSubscriptionEntity>
        {
            public virtual int Compare(EventSubscriptionEntity o1, EventSubscriptionEntity o2)
            {
                return o2.Created.CompareTo(o1.Created);
            }
        }

        private class TreeVisitorAnonymousInnerClass : ITreeVisitor<ScopeImpl>
        {
            private readonly IDictionary<ScopeImpl, PvmExecutionImpl> _scopeExecutionMapping;
            private IActivityExecution _execution;
            private ISet<EventSubscriptionEntity> _subscriptions;

            public TreeVisitorAnonymousInnerClass(IActivityExecution execution,
                IDictionary<ScopeImpl, PvmExecutionImpl> scopeExecutionMapping,
                ISet<EventSubscriptionEntity> subscriptions)
            {
                this._execution = execution;
                this._scopeExecutionMapping = scopeExecutionMapping;
                this._subscriptions = subscriptions;
            }

            public virtual void Visit(ScopeImpl obj)
            {
                var execution = _scopeExecutionMapping[obj];
                _subscriptions.AddAll(((ExecutionEntity)execution).CompensateEventSubscriptions);
            }
        }
    }
}