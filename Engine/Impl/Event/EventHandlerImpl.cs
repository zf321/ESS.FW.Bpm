using System.Collections;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;

namespace ESS.FW.Bpm.Engine.Impl.Event
{
    

    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    public class EventHandlerImpl : IEventHandler
    {
        private readonly EventType _eventType;

        public EventHandlerImpl(EventType eventType)
        {
            this._eventType = eventType;
        }

        public virtual void HandleEvent(EventSubscriptionEntity eventSubscription, object payload,
            CommandContext commandContext)
        {
            HandleIntermediateEvent(eventSubscription, payload, commandContext);
        }

        public virtual string EventHandlerType
        {
            get { return _eventType.Name; }
        }

        public virtual void HandleIntermediateEvent(EventSubscriptionEntity eventSubscription, object payload,
            CommandContext commandContext)
        {
            PvmExecutionImpl execution = eventSubscription.Execution;
            ActivityImpl activity = eventSubscription.Activity;

            EnsureUtil.EnsureNotNull("Error while sending signal for event subscription '" + eventSubscription.Id + "': " + "no activity associated with event subscription", "activity", activity);

            if (payload is IDictionary)
            {
                var processVariables = payload as IDictionary<string, object>;
                execution.Variables = processVariables;
            }

            if (activity==execution.Activity)
            {
                execution.Signal("signal", null);
            }
            else
            {
                // hack around the fact that the start event is refrenced by event subscriptions for event subprocesses
                // and not the subprocess itself
                if (activity.ActivityBehavior is EventSubProcessStartEventActivityBehavior)
                {
                    activity = (ActivityImpl)activity.FlowScope;
                }

                execution.ExecuteEventHandlerActivity(activity);
            }
        }
    }
}