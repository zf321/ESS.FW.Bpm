using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Util
{
    /// <summary>
    /// </summary>
    public class TimerEventFactory : BpmnEventFactory
    {
        public const string TIMER_DATE = "2016-02-11T12:13:14Z";

        public virtual IMigratingBpmnEventTrigger AddBoundaryEvent(IProcessEngine engine,
            IBpmnModelInstance modelInstance, string activityId, string boundaryEventId)
        {
            ModifiableBpmnModelInstance.Wrap(modelInstance)
                //.ActivityBuilder(activityId)
                //.BoundaryEvent(boundaryEventId)
                //.TimerWithDate(TIMER_DATE)
                //.Done()
                ;

            var trigger = new TimerEventTrigger();
            trigger.Engine = engine;
            trigger.ActivityId = boundaryEventId;
            trigger.HandlerType = TimerExecuteNestedActivityJobHandler.TYPE;

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
                //.TimerWithDuration("PT10M")
                .SubProcessDone()
                .Done();

            var trigger = new TimerEventTrigger();
            trigger.Engine = engine;
            trigger.ActivityId = startEventId;
            trigger.HandlerType = TimerStartEventSubprocessJobHandler.TYPE;

            return trigger;
        }


        protected internal class TimerEventTrigger : IMigratingBpmnEventTrigger
        {
            protected internal string ActivityId;

            protected internal IProcessEngine Engine;
            protected internal string HandlerType;

            public virtual void Trigger(string ProcessInstanceId)
            {
                var managementService = Engine.ManagementService;
                var timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId ==ProcessInstanceId)
                    //.ActivityId(ActivityId)
                    .First();

                if (timerJob == null)
                    throw new ProcessEngineException("No job for this event found in context of process instance " +
                                                     ProcessInstanceId);

                managementService.ExecuteJob(timerJob.Id);
            }

            public virtual void AssertEventTriggerMigrated(MigrationTestRule migrationContext, string targetActivityId)
            {
                migrationContext.AssertJobMigrated(ActivityId, targetActivityId, HandlerType);
            }

            public virtual IMigratingBpmnEventTrigger InContextOf(string newActivityId)
            {
                var newTrigger = new TimerEventTrigger();
                newTrigger.ActivityId = newActivityId;
                newTrigger.Engine = Engine;
                newTrigger.HandlerType = HandlerType;
                return newTrigger;
            }
        }
    }
}