using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Signal
{
    [TestFixture]
    public class SignalEventExpressionNameTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testSignalCatchIntermediate()
        {

            // given
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["var"] = "TestVar";

            // when
            runtimeService.StartProcessInstanceByKey("catchSignal", variables);

            // then
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert-TestVar").Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventExpressionNameTest.testSignalCatchIntermediate.bpmn20.xml" })]
        public virtual void testSignalCatchIntermediateActsOnEventReceive()
        {

            // given
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["var"] = "TestVar";

            // when
            runtimeService.StartProcessInstanceByKey("catchSignal", variables);
            runtimeService.SignalEventReceived("alert-TestVar");

            // then
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert-TestVar").Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventExpressionNameTest.testSignalCatchIntermediate.bpmn20.xml", "resources/bpmn/event/signal/SignalEventExpressionNameTest.testSignalThrowIntermediate.bpmn20.xml" })]
        public virtual void testSignalThrowCatchIntermediate()
        {

            // given
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["var"] = "TestVar";

            // when
            runtimeService.StartProcessInstanceByKey("catchSignal", variables);
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert-TestVar").Count());
            runtimeService.StartProcessInstanceByKey("throwSignal", variables);

            // then
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert-${var}").Count());
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert-TestVar").Count());
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventExpressionNameTest.testSignalCatchIntermediate.bpmn20.xml", "resources/bpmn/event/signal/SignalEventExpressionNameTest.testSignalThrowEnd.bpmn20.xml" })]
        public virtual void testSignalThrowEndCatchIntermediate()
        {

            // given
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["var"] = "TestVar";

            // when
            runtimeService.StartProcessInstanceByKey("catchSignal", variables);
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert-TestVar").Count());
            runtimeService.StartProcessInstanceByKey("throwEndSignal", variables);

            // then
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert-${var}").Count());
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert-TestVar").Count());
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventExpressionNameTest.testSignalCatchBoundary.bpmn20.xml", "resources/bpmn/event/signal/SignalEventExpressionNameTest.testSignalThrowIntermediate.bpmn20.xml" })]
        public virtual void testSignalCatchBoundary()
        {

            // given
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["var"] = "TestVar";
            runtimeService.StartProcessInstanceByKey("catchSignal", variables);
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert-TestVar").Count());
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());

            // when
            runtimeService.StartProcessInstanceByKey("throwSignal", variables);

            // then
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert-TestVar").Count());
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventExpressionNameTest.testSignalStartEvent.bpmn20.xml" })]
        public virtual void testSignalStartEvent()
        {

            // given
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert-foo").Count());
            Assert.AreEqual(0, taskService.CreateTaskQuery().Count());

            // when
            runtimeService.SignalEventReceived("alert-foo");

            // then
            // the signal should start a new process instance
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testSignalStartEventInEventSubProcess()
        {

            // given
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("signalStartEventInEventSubProcess");
            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==processInstance.Id);
            Assert.AreEqual(1, executionQuery.Count());
            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id);
            Assert.AreEqual(1, taskQuery.Count());

            // when
            runtimeService.SignalEventReceived("alert-foo");

            // then
            Assert.AreEqual(true, DummyServiceTask.wasExecuted);
            // check if IUser task doesn't exist because signal start event is interrupting
            taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id);
            Assert.AreEqual(0, taskQuery.Count());
            // check if execution doesn't exist because signal start event is interrupting
            executionQuery = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==processInstance.Id);
            Assert.AreEqual(0, executionQuery.Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventExpressionNameTest.testSignalStartEvent.bpmn20.xml", "resources/bpmn/event/signal/SignalEventExpressionNameTest.throwAlertSignalAsync.bpmn20.xml" })]
        public virtual void testAsyncSignalStartEvent()
        {
            IProcessDefinition catchingProcessDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "startBySignal").First();

            // given a process instance that throws a signal asynchronously
            runtimeService.StartProcessInstanceByKey("throwSignalAsync");
            // with an async job to trigger the signal event
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            // when the job is executed
            managementService.ExecuteJob(job.Id);

            // then there is a process instance
            IProcessInstance processInstance = runtimeService.CreateProcessInstanceQuery().First();
            Assert.NotNull(processInstance);
            Assert.AreEqual(catchingProcessDefinition.Id, processInstance.ProcessDefinitionId);

            // and a task
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventExpressionNameTest.testSignalCatchIntermediate.bpmn20.xml" })]
        public virtual void testSignalExpressionErrorHandling()
        {

            string expectedErrorMessage = "Unknown property used in expression: alert-${var}. Cannot resolve identifier 'var'";

            // given an empty variable mapping
            Dictionary<string, object> variables = new Dictionary<string, object>();

            try
            {
                // when starting the process
                runtimeService.StartProcessInstanceByKey("catchSignal", variables);

                Assert.Fail("exception expected: " + expectedErrorMessage);
            }
            catch (ProcessEngineException)
            {
                // then the expression cannot be resolved and no signal should be available
                Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal").Count());
            }
        }

    }

}