using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Util
{
    /// <summary>
    /// </summary>
    public class SignalEventFactory : BpmnEventFactory
    {
        public const string SIGNAL_NAME = "signal";

        public virtual IMigratingBpmnEventTrigger AddBoundaryEvent(IProcessEngine engine,
            IBpmnModelInstance modelInstance, string activityId, string boundaryEventId)
        {
            ModifiableBpmnModelInstance.Wrap(modelInstance)
                //.ActivityBuilder(activityId)
                //.BoundaryEvent(boundaryEventId)
                //.Signal(SIGNAL_NAME)
                //.Done()
                ;

            var trigger = new SignalTrigger();
            trigger.engine = engine;
            trigger.signalName = SIGNAL_NAME;
            trigger.activityId = boundaryEventId;

            return trigger;
        }

        public virtual IMigratingBpmnEventTrigger AddEventSubProcess(IProcessEngine engine,
            IBpmnModelInstance modelInstance, string parentId, string subProcessId, string startEventId)
        {
            ModifiableBpmnModelInstance.Wrap(modelInstance)
                .AddSubProcessTo(parentId)
                //.Id(subProcessId)
                .TriggerByEvent()
                ////.EmbeddedSubProcess()
                //.StartEvent(startEventId)
                //.Signal(SIGNAL_NAME)
                .SubProcessDone()
                .Done();

            var trigger = new SignalTrigger();
            trigger.engine = engine;
            trigger.signalName = SIGNAL_NAME;
            trigger.activityId = startEventId;

            return trigger;
        }

        protected internal class SignalTrigger : IMigratingBpmnEventTrigger
        {
            protected internal string activityId;

            protected internal IProcessEngine engine;
            protected internal string signalName;

            public virtual void Trigger(string ProcessInstanceId)
            {
                var eventSubscription = engine.RuntimeService.CreateEventSubscriptionQuery(c => c.ActivityId == activityId)
                    //.EventName(signalName)
                    .Where(c => c.ProcessInstanceId == ProcessInstanceId)
                    .First();

                if (eventSubscription == null)
                    throw new System.Exception("IEvent subscription not found");

                engine.RuntimeService.SignalEventReceived(eventSubscription.EventName, eventSubscription.ExecutionId);
            }

            public virtual void AssertEventTriggerMigrated(MigrationTestRule migrationContext, string targetActivityId)
            {
                migrationContext.AssertEventSubscriptionMigrated(activityId, targetActivityId, SIGNAL_NAME);
            }

            public virtual IMigratingBpmnEventTrigger InContextOf(string newActivityId)
            {
                var newTrigger = new SignalTrigger();
                newTrigger.activityId = newActivityId;
                newTrigger.engine = engine;
                newTrigger.signalName = signalName;
                return newTrigger;
            }
        }
    }
}