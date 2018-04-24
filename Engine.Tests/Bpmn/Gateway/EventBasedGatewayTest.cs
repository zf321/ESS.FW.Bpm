using System;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Gateway
{
    [TestFixture]
    public class EventBasedGatewayTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment(new string[] { "resources/bpmn/gateway/EventBasedGatewayTest.TestCatchAlertAndTimer.bpmn20.xml", "resources/bpmn/gateway/EventBasedGatewayTest.ThrowAlertSignal.bpmn20.xml" })]
        public virtual void testCatchSignalCancelsTimer()
        {
             
            runtimeService.StartProcessInstanceByKey("catchSignal");
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());

            runtimeService.StartProcessInstanceByKey("throwSignal");

            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());
            Assert.AreEqual(0, managementService.CreateJobQuery().Count());

            ITask task = taskService.CreateTaskQuery(c=>c.Name =="afterSignal").First();

            Assert.NotNull(task);

            taskService.Complete(task.Id);

        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/gateway/EventBasedGatewayTest.TestCatchAlertAndTimer.bpmn20.xml" })]
        public virtual void testCatchTimerCancelsSignal()
        {

            runtimeService.StartProcessInstanceByKey("catchSignal");

            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());

            ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Millisecond + 10000);
            try
            {
                // wait for timer to fire
                WaitForJobExecutorToProcessAllJobs(10000);

                Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());
                Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());
                Assert.AreEqual(0, managementService.CreateJobQuery().Count());

                ITask task = taskService.CreateTaskQuery(c=>c.Name =="afterTimer").First();

                Assert.NotNull(task);

                taskService.Complete(task.Id);
            }
            finally
            {
                ClockUtil.CurrentTime = DateTime.Now;
            }
        }

        [Test]
        [Deployment]
        public virtual void testCatchSignalAndMessageAndTimer()
        {

            runtimeService.StartProcessInstanceByKey("catchSignal");

            Assert.AreEqual(2, runtimeService.CreateEventSubscriptionQuery().Count());
            IQueryable<IEventSubscription> messageEventSubscriptionQuery = runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "message");
            Assert.AreEqual(1, messageEventSubscriptionQuery.Count());
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal").Count());
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());

            // we can query for an execution with has both a signal AND message subscription
            IExecution execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("newInvoice")*//*.SignalEventSubscriptionName("alert")*/.First();
            Assert.NotNull(execution);

            ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Millisecond + 10000);
            try
            {

                IEventSubscription messageEventSubscription = messageEventSubscriptionQuery.First();
                runtimeService.MessageEventReceived(messageEventSubscription.EventName, messageEventSubscription.ExecutionId);

                Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());
                Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());
                Assert.AreEqual(0, managementService.CreateJobQuery().Count());

                ITask task = taskService.CreateTaskQuery(c=>c.Name =="afterMessage").First();

                Assert.NotNull(task);

                taskService.Complete(task.Id);
            }
            finally
            {
                ClockUtil.CurrentTime = DateTime.Now;
            }
        }

        public virtual void testConnectedToActitiy()
        {

            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/gateway/EventBasedGatewayTest.TestConnectedToActivity.bpmn20.xml").Deploy();
                Assert.Fail("exception expected");
            }
            catch (System.Exception e)
            {
                if (!e.Message.Contains("Event based gateway can only be connected to elements of type intermediateCatchEvent"))
                {
                    Assert.Fail("different exception expected");
                }
            }

        }

        public virtual void testInvalidSequenceFlow()
        {

            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/gateway/EventBasedGatewayTest.TestEventInvalidSequenceFlow.bpmn20.xml").Deploy();
                Assert.Fail("exception expected");
            }
            catch (System.Exception e)
            {
                if (!e.Message.Contains("Invalid incoming sequenceflow for intermediateCatchEvent"))
                {
                    Assert.Fail("different exception expected");
                }
            }

        }

        [Test]
        [Deployment]
        public virtual void testTimeCycle()
        {
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            string jobId = jobQuery.First().Id;
            managementService.ExecuteJob(jobId);

            Assert.AreEqual(0, jobQuery.Count());

            string taskId = taskService.CreateTaskQuery().First().Id;
            taskService.Complete(taskId);

            AssertProcessEnded(ProcessInstanceId);
        }

    }

}