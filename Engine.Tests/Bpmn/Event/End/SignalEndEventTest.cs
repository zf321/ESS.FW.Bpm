using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.End
{
    [TestFixture]
    public class SignalEndEventTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testCatchSignalEndEventInEmbeddedSubprocess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("catchSignalEndEventInEmbeddedSubprocess");
            Assert.NotNull(processInstance);

            // After process start, usertask in subprocess should exist
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("subprocessTask", task.Name);

            // After task completion, signal end event is reached and caught
            taskService.Complete(task.Id);

            task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("task after catching the signal", task.Name);

            taskService.Complete(task.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/end/SignalEndEventTest.catchSignalEndEvent.bpmn20.xml", "resources/bpmn/event/end/SignalEndEventTest.processWithSignalEndEvent.bpmn20.xml" })]
        public virtual void testCatchSignalEndEventInCallActivity()
        {
            // first, start process to wait of the signal event
            IProcessInstance processInstanceCatchEvent = runtimeService.StartProcessInstanceByKey("catchSignalEndEvent");
            Assert.NotNull(processInstanceCatchEvent);

            // now we have a subscription for the signal event:
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery().Count());
            Assert.AreEqual("alert", runtimeService.CreateEventSubscriptionQuery().First().EventName);

            // start process which throw the signal end event
            IProcessInstance processInstanceEndEvent = runtimeService.StartProcessInstanceByKey("processWithSignalEndEvent");
            Assert.NotNull(processInstanceEndEvent);
            AssertProcessEnded(processInstanceEndEvent.Id);

            // User task of process catchSignalEndEvent
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("taskAfterSignalCatch", task.TaskDefinitionKey);

            // complete User task
            taskService.Complete(task.Id);

            AssertProcessEnded(processInstanceCatchEvent.Id);
        }

    }

}