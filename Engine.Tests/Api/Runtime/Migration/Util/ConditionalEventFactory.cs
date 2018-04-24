using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Util
{
    /// <summary>
    /// </summary>
    public class ConditionalEventFactory : BpmnEventFactory
    {
        protected internal const string VAR_CONDITION = "${any=='any'}";

        public virtual IMigratingBpmnEventTrigger AddBoundaryEvent(IProcessEngine engine,
            IBpmnModelInstance modelInstance, string activityId, string boundaryEventId)
        {
            ModifiableBpmnModelInstance.Wrap(modelInstance)
                //.ActivityBuilder(activityId)
                //.BoundaryEvent(boundaryEventId)
                //.Condition(VAR_CONDITION)
                //.Done()
                ;

            var trigger = new ConditionalEventTrigger();
            trigger.Engine = engine;
            trigger.VariableName = "any";
            trigger.VariableValue = "any";
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
                //.Condition(VAR_CONDITION)
                .SubProcessDone()
                .Done();

            var trigger = new ConditionalEventTrigger();
            trigger.Engine = engine;
            trigger.VariableName = "any";
            trigger.VariableValue = "any";
            trigger.ActivityId = startEventId;

            return trigger;
        }

        protected internal class ConditionalEventTrigger : IMigratingBpmnEventTrigger
        {
            protected internal string ActivityId;

            protected internal IProcessEngine Engine;
            protected internal string VariableName;
            protected internal object VariableValue;

            public virtual void Trigger(string ProcessInstanceId)
            {
                Engine.RuntimeService.SetVariable(ProcessInstanceId, VariableName, VariableValue);
            }

            public virtual void AssertEventTriggerMigrated(MigrationTestRule migrationContext, string targetActivityId)
            {
                migrationContext.AssertEventSubscriptionMigrated(ActivityId, targetActivityId, null);
            }

            public virtual IMigratingBpmnEventTrigger InContextOf(string newActivityId)
            {
                var newTrigger = new ConditionalEventTrigger();
                newTrigger.ActivityId = newActivityId;
                newTrigger.Engine = Engine;
                newTrigger.VariableName = VariableName;
                newTrigger.VariableValue = VariableValue;
                return newTrigger;
            }
        }
    }
}