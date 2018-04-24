using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Util
{
    /// <summary>
    /// </summary>
    public class MessageEventFactory : BpmnEventFactory
    {
        public const string MESSAGE_NAME = "message";

        public virtual IMigratingBpmnEventTrigger AddBoundaryEvent(IProcessEngine engine,
            IBpmnModelInstance modelInstance, string activityId, string boundaryEventId)
        {
            ModifiableBpmnModelInstance.Wrap(modelInstance)
                //.ActivityBuilder(activityId)
                //.BoundaryEvent(boundaryEventId)
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;

            var trigger = new MessageTrigger();
            trigger.Engine = engine;
            trigger.MessageName = MESSAGE_NAME;
            trigger.ActivityId = boundaryEventId;

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
                ////.Message(MESSAGE_NAME)
                .SubProcessDone()
                .Done();

            var trigger = new MessageTrigger();
            trigger.Engine = engine;
            trigger.MessageName = MESSAGE_NAME;
            trigger.ActivityId = startEventId;

            return trigger;
        }


        protected internal class MessageTrigger : IMigratingBpmnEventTrigger
        {
            protected internal string ActivityId;

            protected internal IProcessEngine Engine;
            protected internal string MessageName;

            public virtual void Trigger(string ProcessInstanceId)
            {
                Engine.RuntimeService.CreateMessageCorrelation(MessageName)
                    .ProcessInstanceId(ProcessInstanceId)
                    .CorrelateWithResult();
            }

            public virtual void AssertEventTriggerMigrated(MigrationTestRule migrationContext, string targetActivityId)
            {
                migrationContext.AssertEventSubscriptionMigrated(ActivityId, targetActivityId, MessageName);
            }

            public virtual IMigratingBpmnEventTrigger InContextOf(string newActivityId)
            {
                var newTrigger = new MessageTrigger();
                newTrigger.ActivityId = newActivityId;
                newTrigger.Engine = Engine;
                newTrigger.MessageName = MessageName;
                return newTrigger;
            }
        }
    }
}