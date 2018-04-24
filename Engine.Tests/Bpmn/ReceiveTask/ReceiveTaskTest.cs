using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.ReceiveTask
{
    [TestFixture]
    public class ReceiveTaskTest : PluggableProcessEngineTestCase
    {

        private IList<IEventSubscription> EventSubscriptionList
        {
            get
            {var query= runtimeService.CreateEventSubscriptionQuery(c => c.EventType == "message").ToList();
                return query;
            }
        }

        private IList<IEventSubscription> getEventSubscriptionList(string activityId)
        {
            return runtimeService.CreateEventSubscriptionQuery(c => c.EventType == "message" && c.ActivityId == activityId).ToList();
        }

        private string getExecutionId(string ProcessInstanceId, string activityId)
        {
            return runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == ProcessInstanceId && c.ActivityId == activityId).First().Id;
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.simpleReceiveTask.bpmn20.xml" })]
        
        public virtual void testReceiveTaskWithoutMessageReference()
        {
            // given: a process instance waiting in the receive task
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there is no message event subscription created for a receive task without a message reference
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // then: we can signal the waiting receive task
            runtimeService.Signal(processInstance.Id);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.singleReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsLegacySignalingOnSingleReceiveTask()
        {
            // given: a process instance waiting in the receive task
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there is a message event subscription for the task
            Assert.AreEqual(1, EventSubscriptionList.Count);

            // then: we can signal the waiting receive task
            runtimeService.Signal(getExecutionId(processInstance.Id, "waitState"));

            // expect: subscription is removed
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.singleReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsMessageEventReceivedOnSingleReceiveTask()
        {

            // given: a process instance waiting in the receive task
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there is a message event subscription for the task
            IList<IEventSubscription> subscriptionList = EventSubscriptionList;
            Assert.AreEqual(1, subscriptionList.Count);
            IEventSubscription subscription = subscriptionList[0];

            // then: we can trigger the event subscription
            runtimeService.MessageEventReceived(subscription.EventName, subscription.ExecutionId);

            // expect: subscription is removed
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.singleReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsCorrelateMessageOnSingleReceiveTask()
        {

            // given: a process instance waiting in the receive task
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there is a message event subscription for the task
            IList<IEventSubscription> subscriptionList = EventSubscriptionList;
            Assert.AreEqual(1, subscriptionList.Count);
            IEventSubscription subscription = subscriptionList[0];

            // then: we can correlate the event subscription
            runtimeService.CorrelateMessage(subscription.EventName);

            // expect: subscription is removed
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.singleReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsCorrelateMessageByBusinessKeyOnSingleReceiveTask()
        { 

            // given: a process instance with business key 23 waiting in the receive task
            IProcessInstance processInstance23 = runtimeService.StartProcessInstanceByKey("testProcess", "23");

            // given: a 2nd process instance with business key 42 waiting in the receive task
            IProcessInstance processInstance42 = runtimeService.StartProcessInstanceByKey("testProcess", "42");

            // expect: there is two message event subscriptions for the tasks
            IList<IEventSubscription> subscriptionList = EventSubscriptionList;
            Assert.AreEqual(2, subscriptionList.Count);

            // then: we can correlate the event subscription to one of the process instances
            runtimeService.CorrelateMessage("newInvoiceMessage", "23");

            // expect: one subscription is removed
            Assert.AreEqual(1, EventSubscriptionList.Count);

            // expect: this ends the process instance with business key 23
            AssertProcessEnded(processInstance23.Id);

            // expect: other process instance is still running
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery(c => c.ProcessInstanceId == processInstance42.Id).Count());

            // then: we can correlate the event subscription to the other process instance
            runtimeService.CorrelateMessage("newInvoiceMessage", "42");

            // expect: subscription is removed
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance42.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsLegacySignalingOnSequentialMultiReceiveTask()
        {

            // given: a process instance waiting in the first receive tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there is a message event subscription for the first task
            IList<IEventSubscription> subscriptionList = EventSubscriptionList;
            Assert.AreEqual(1, subscriptionList.Count);
            IEventSubscription subscription = subscriptionList[0];
            string firstSubscriptionId = subscription.Id;

            // then: we can signal the waiting receive task
            runtimeService.Signal(getExecutionId(processInstance.Id, "waitState"));

            // expect: there is a new subscription created for the second receive task instance
            subscriptionList = EventSubscriptionList;
            Assert.AreEqual(1, subscriptionList.Count);
            subscription = subscriptionList[0];
            Assert.IsFalse(firstSubscriptionId.Equals(subscription.Id));

            // then: we can signal the second waiting receive task
            runtimeService.Signal(getExecutionId(processInstance.Id, "waitState"));

            // expect: no event subscription left
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: one IUser task is created
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsMessageEventReceivedOnSequentialMultiReceiveTask()
        {

            // given: a process instance waiting in the first receive tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there is a message event subscription for the first task
            IList<IEventSubscription> subscriptionList = EventSubscriptionList;
            Assert.AreEqual(1, subscriptionList.Count);
            IEventSubscription subscription = subscriptionList[0];
            string firstSubscriptionId = subscription.Id;

            // then: we can trigger the event subscription
            runtimeService.MessageEventReceived(subscription.EventName, subscription.ExecutionId);

            // expect: there is a new subscription created for the second receive task instance
            subscriptionList = EventSubscriptionList;
            Assert.AreEqual(1, subscriptionList.Count);
            subscription = subscriptionList[0];
            Assert.IsFalse(firstSubscriptionId.Equals(subscription.Id));

            // then: we can trigger the second event subscription
            runtimeService.MessageEventReceived(subscription.EventName, subscription.ExecutionId);

            // expect: no event subscription left
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: one IUser task is created
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsCorrelateMessageOnSequentialMultiReceiveTask()
        {

            // given: a process instance waiting in the first receive tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there is a message event subscription for the first task
            IList<IEventSubscription> subscriptionList = EventSubscriptionList;
            Assert.AreEqual(1, subscriptionList.Count);
            IEventSubscription subscription = subscriptionList[0];
            string firstSubscriptionId = subscription.Id;

            // then: we can trigger the event subscription
            runtimeService.CorrelateMessage(subscription.EventName);

            // expect: there is a new subscription created for the second receive task instance
            subscriptionList = EventSubscriptionList;
            Assert.AreEqual(1, subscriptionList.Count);
            subscription = subscriptionList[0];
            Assert.IsFalse(firstSubscriptionId.Equals(subscription.Id));

            // then: we can trigger the second event subscription
            runtimeService.CorrelateMessage(subscription.EventName);

            // expect: no event subscription left
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: one IUser task is created
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsLegacySignalingOnParallelMultiReceiveTask()
        {

            // given: a process instance waiting in two receive tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there are two message event subscriptions
            IList<IEventSubscription> subscriptions = EventSubscriptionList;
            Assert.AreEqual(2, subscriptions.Count);

            // expect: there are two executions
            IList<IExecution> executions = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id && c.ActivityId == "waitState")/*.MessageEventSubscriptionName("newInvoiceMessage")*/.ToList();
            Assert.AreEqual(2, executions.Count);

            // then: we can signal both waiting receive task
            runtimeService.Signal(executions[0].Id);
            runtimeService.Signal(executions[1].Id);

            // expect: both event subscriptions are removed
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsMessageEventReceivedOnParallelMultiReceiveTask()
        {

            // given: a process instance waiting in two receive tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there are two message event subscriptions
            IList<IEventSubscription> subscriptions = EventSubscriptionList;
            Assert.AreEqual(2, subscriptions.Count);

            // then: we can trigger both event subscriptions
            runtimeService.MessageEventReceived(subscriptions[0].EventName, subscriptions[0].ExecutionId);
            runtimeService.MessageEventReceived(subscriptions[1].EventName, subscriptions[1].ExecutionId);

            // expect: both event subscriptions are removed
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml" })]
        public virtual void testNotSupportsCorrelateMessageOnParallelMultiReceiveTask()
        {

            // given: a process instance waiting in two receive tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there are two message event subscriptions
            IList<IEventSubscription> subscriptions = EventSubscriptionList;
            Assert.AreEqual(2, subscriptions.Count);

            // then: we can not correlate an event
            try
            {
                runtimeService.CorrelateMessage(subscriptions[0].EventName);
                Assert.Fail("should throw a mismatch");
            }
            catch (MismatchingMessageCorrelationException)
            {
                // expected
            }
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsMessageEventReceivedOnParallelMultiReceiveTaskWithCompensation()
        {

            // given: a process instance waiting in two receive tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there are two message event subscriptions
            IList<IEventSubscription> subscriptions = EventSubscriptionList;
            Assert.AreEqual(2, subscriptions.Count);

            // then: we can trigger the first event subscription
            runtimeService.MessageEventReceived(subscriptions[0].EventName, subscriptions[0].ExecutionId);

            // expect: after completing the first receive task there is one event subscription for compensation
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery(c => c.EventType == EventType.Compensate.Name).Count());

            // then: we can trigger the second event subscription
            runtimeService.MessageEventReceived(subscriptions[1].EventName, subscriptions[1].ExecutionId);

            // expect: there are three event subscriptions for compensation (two subscriptions for tasks and one for miBody)
            Assert.AreEqual(3, runtimeService.CreateEventSubscriptionQuery(c => c.EventType == EventType.Compensate.Name).Count());

            // expect: one IUser task is created
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsMessageEventReceivedOnParallelMultiInstanceWithBoundary()
        {

            // given: a process instance waiting in two receive tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there are three message event subscriptions
            Assert.AreEqual(3, EventSubscriptionList.Count);

            // expect: there are two message event subscriptions for the receive tasks
            IList<IEventSubscription> subscriptions = getEventSubscriptionList("waitState");
            Assert.AreEqual(2, subscriptions.Count);

            // then: we can trigger both receive task event subscriptions
            runtimeService.MessageEventReceived(subscriptions[0].EventName, subscriptions[0].ExecutionId);
            runtimeService.MessageEventReceived(subscriptions[1].EventName, subscriptions[1].ExecutionId);

            // expect: all subscriptions are removed (boundary subscription is removed too)
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsMessageEventReceivedOnParallelMultiInstanceWithBoundaryEventReceived()
        {

            // given: a process instance waiting in two receive tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there are three message event subscriptions
            Assert.AreEqual(3, EventSubscriptionList.Count);

            // expect: there is one message event subscription for the boundary event
            IList<IEventSubscription> subscriptions = getEventSubscriptionList("cancel");
            Assert.AreEqual(1, subscriptions.Count);
            IEventSubscription subscription = subscriptions[0];

            // then: we can trigger the boundary subscription to cancel both tasks
            runtimeService.MessageEventReceived(subscription.EventName, subscription.ExecutionId);

            // expect: all subscriptions are removed (receive task subscriptions too)
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsMessageEventReceivedOnSubProcessReceiveTask()
        {

            // given: a process instance waiting in the sub-process receive task
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there is a message event subscription for the task
            IList<IEventSubscription> subscriptionList = EventSubscriptionList;
            Assert.AreEqual(1, subscriptionList.Count);
            IEventSubscription subscription = subscriptionList[0];

            // then: we can trigger the event subscription
            runtimeService.MessageEventReceived(subscription.EventName, subscription.ExecutionId);

            // expect: subscription is removed
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.multiSequentialReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsMessageEventReceivedOnMultiSubProcessReceiveTask()
        {

            // given: a process instance waiting in two parallel sub-process receive tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there are two message event subscriptions
            IList<IEventSubscription> subscriptions = EventSubscriptionList;
            Assert.AreEqual(2, subscriptions.Count);

            // then: we can trigger both receive task event subscriptions
            runtimeService.MessageEventReceived(subscriptions[0].EventName, subscriptions[0].ExecutionId);
            runtimeService.MessageEventReceived(subscriptions[1].EventName, subscriptions[1].ExecutionId);

            // expect: subscriptions are removed
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.parallelGatewayReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsMessageEventReceivedOnReceiveTaskBehindParallelGateway()
        {

            // given: a process instance waiting in two receive tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there are two message event subscriptions
            IList<IEventSubscription> subscriptions = EventSubscriptionList;
            Assert.AreEqual(2, subscriptions.Count);

            // then: we can trigger both receive task event subscriptions
            runtimeService.MessageEventReceived(subscriptions[0].EventName, subscriptions[0].ExecutionId);
            runtimeService.MessageEventReceived(subscriptions[1].EventName, subscriptions[1].ExecutionId);

            // expect: subscriptions are removed
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/receivetask/ReceiveTaskTest.parallelGatewayReceiveTask.bpmn20.xml" })]
        public virtual void testSupportsCorrelateMessageOnReceiveTaskBehindParallelGateway()
        {

            // given: a process instance waiting in two receive tasks
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // expect: there are two message event subscriptions
            IList<IEventSubscription> subscriptions = EventSubscriptionList;
            Assert.AreEqual(2, subscriptions.Count);

            // then: we can trigger both receive task event subscriptions
            runtimeService.CorrelateMessage(subscriptions[0].EventName);
            runtimeService.CorrelateMessage(subscriptions[1].EventName);

            // expect: subscriptions are removed
            Assert.AreEqual(0, EventSubscriptionList.Count);

            // expect: this ends the process instance
            AssertProcessEnded(processInstance.Id);
        }


        [Test]
        public virtual void testWaitStateBehavior()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("receiveTask");
            IExecution execution = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi.Id && c.ActivityId == "waitState").First();
            Assert.NotNull(execution);

            runtimeService.Signal(execution.Id);
            AssertProcessEnded(pi.Id);
        }
    }

}