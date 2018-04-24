using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Parse
{

    [TestFixture]
    public class FoxFailedJobParseListenerTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/parse/FoxFailedJobParseListenerTest.TestUserTask.bpmn20.xml" })]
        public virtual void TestUserTaskParseFailedJobRetryTimeCycle()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("asyncUserTaskFailedJobRetryTimeCycle");

            ActivityImpl userTask = FindActivity(pi, "task");
            CheckFoxFailedJobConfig(userTask);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/parse/CamundaFailedJobParseListenerTest.TestUserTask.bpmn20.xml" })]
        public virtual void TestUserTaskParseFailedJobRetryTimeCycleInActivitiNamespace()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("asyncUserTaskFailedJobRetryTimeCycle");

            ActivityImpl userTask = FindActivity(pi, "task");
            CheckFoxFailedJobConfig(userTask);
        }

        [Test]
        [Deployment(Resources =new string[] { "resources/bpmn/parse/FoxFailedJobParseListenerTest.TestUserTask.bpmn20.xml" })]
        public virtual void TestNotAsyncUserTaskParseFailedJobRetryTimeCycle()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("notAsyncUserTaskFailedJobRetryTimeCycle");

            ActivityImpl userTask = FindActivity(pi, "notAsyncTask");
            CheckNotContainingFoxFailedJobConfig(userTask);
        }

        [Test]
        [Deployment(Resources =new string[] { "resources/bpmn/parse/FoxFailedJobParseListenerTest.TestUserTask.bpmn20.xml" })]
        public virtual void TestAsyncUserTaskButWithoutParseFailedJobRetryTimeCycle()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("asyncUserTaskButWithoutFailedJobRetryTimeCycle");

            ActivityImpl userTask = FindActivity(pi, "asyncTaskWithoutFailedJobRetryTimeCycle");
            CheckNotContainingFoxFailedJobConfig(userTask);
        }

        [Test]
        [Deployment(Resources =new string[] { "resources/bpmn/parse/FoxFailedJobParseListenerTest.TestTimer.bpmn20.xml" })]
        public virtual void TestTimerBoundaryEventWithFailedJobRetryTimeCycle()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("boundaryEventWithFailedJobRetryTimeCycle");

            ActivityImpl boundaryActivity = FindActivity(pi, "boundaryTimerWithFailedJobRetryTimeCycle");
            CheckFoxFailedJobConfig(boundaryActivity);
        }

        [Test]
        [Deployment(Resources =new string[] { "resources/bpmn/parse/FoxFailedJobParseListenerTest.TestTimer.bpmn20.xml" })]
        public virtual void TestTimerBoundaryEventWithoutFailedJobRetryTimeCycle()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("boundaryEventWithoutFailedJobRetryTimeCycle");

            ActivityImpl boundaryActivity = FindActivity(pi, "boundaryTimerWithoutFailedJobRetryTimeCycle");
            CheckNotContainingFoxFailedJobConfig(boundaryActivity);
        }

        [Test]
        [Deployment(Resources =new string[] { "resources/bpmn/parse/FoxFailedJobParseListenerTest.TestTimer.bpmn20.xml" })]
        public virtual void TestTimerStartEventWithFailedJobRetryTimeCycle()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("startEventWithFailedJobRetryTimeCycle");

            ActivityImpl startEvent = FindActivity(pi, "startEventFailedJobRetryTimeCycle");
            CheckFoxFailedJobConfig(startEvent);
        }

        [Test]
        [Deployment(Resources =new string[] { "resources/bpmn/parse/FoxFailedJobParseListenerTest.TestTimer.bpmn20.xml" })]
        public virtual void TestIntermediateCatchTimerEventWithFailedJobRetryTimeCycle()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("intermediateTimerEventWithFailedJobRetryTimeCycle");

            ActivityImpl timer = FindActivity(pi, "timerEventWithFailedJobRetryTimeCycle");
            CheckFoxFailedJobConfig(timer);
        }

        [Test]
        [Deployment(Resources =new string[] { "resources/bpmn/parse/FoxFailedJobParseListenerTest.TestSignal.bpmn20.xml" })]
        public virtual void TestSignalEventWithFailedJobRetryTimeCycle()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("signalEventWithFailedJobRetryTimeCycle");

            ActivityImpl signal = FindActivity(pi, "signalWithFailedJobRetryTimeCycle");
            CheckFoxFailedJobConfig(signal);
        }

        [Test]
        [Deployment]
        public virtual void TestMultiInstanceBodyWithFailedJobRetryTimeCycle()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("process");

            ActivityImpl miBody = FindMultiInstanceBody(pi, "task");
            CheckFoxFailedJobConfig(miBody);

            ActivityImpl innerActivity = FindActivity(pi, "task");
            CheckNotContainingFoxFailedJobConfig(innerActivity);
        }

        [Test]
        [Deployment]
        public virtual void TestInnerMultiInstanceActivityWithFailedJobRetryTimeCycle()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("process");

            ActivityImpl miBody = FindMultiInstanceBody(pi, "task");
            CheckNotContainingFoxFailedJobConfig(miBody);

            ActivityImpl innerActivity = FindActivity(pi, "task");
            CheckFoxFailedJobConfig(innerActivity);
        }

        [Test]
        [Deployment]
        public virtual void TestMultiInstanceBodyAndInnerActivityWithFailedJobRetryTimeCycle()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("process");

            ActivityImpl miBody = FindMultiInstanceBody(pi, "task");
            CheckFoxFailedJobConfig(miBody);

            ActivityImpl innerActivity = FindActivity(pi, "task");
            CheckFoxFailedJobConfig(innerActivity);
        }

        protected internal virtual ActivityImpl FindActivity(IProcessInstance pi, string activityId)
        {

            ProcessInstanceWithVariablesImpl entity = (ProcessInstanceWithVariablesImpl)pi;
            ProcessDefinitionEntity processDefEntity = (entity.ExecutionEntity as ExecutionEntity).ProcessDefinition as ProcessDefinitionEntity;

            Assert.NotNull(processDefEntity);
            ActivityImpl activity = processDefEntity.FindActivity(activityId) as ActivityImpl;
            Assert.NotNull(activity);
            return activity;
        }

        protected internal virtual ActivityImpl FindMultiInstanceBody(IProcessInstance pi, string activityId)
        {
            return FindActivity(pi, activityId + BpmnParse.MultiInstanceBodyIdSuffix);
        }

        protected internal virtual void CheckFoxFailedJobConfig(ActivityImpl activity)
        {
            Assert.NotNull(activity);

            Assert.True(activity.Properties.Contains(FoxFailedJobParseListener.FoxFailedJobConfiguration));

            object value = activity.Properties.Get(FoxFailedJobParseListener.FoxFailedJobConfiguration);
            Assert.AreEqual("R5/PT5M", value);
        }

        protected internal virtual void CheckNotContainingFoxFailedJobConfig(ActivityImpl activity)
        {
            Assert.IsFalse(activity.Properties.Contains(FoxFailedJobParseListener.FoxFailedJobConfiguration));
        }

    }

}