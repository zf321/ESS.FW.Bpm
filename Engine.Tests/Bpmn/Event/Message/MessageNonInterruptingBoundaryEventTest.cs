using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Message
{
    [TestFixture]
    public class MessageNonInterruptingBoundaryEventTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]        
        public virtual void testSingleNonInterruptingBoundaryMessageEvent()
        {
            runtimeService.StartProcessInstanceByKey("process");

            Assert.AreEqual(2, runtimeService.CreateExecutionQuery().Count());

            ITask userTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            Assert.NotNull(userTask);

            IExecution execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.NotNull(execution);

            // 1. case: message received before completing the task

            runtimeService.MessageEventReceived("messageName", execution.Id);
            // event subscription not removed
            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.NotNull(execution);

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            userTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskAfterMessage").First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterMessage", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());

            // send a message a second time
            runtimeService.MessageEventReceived("messageName", execution.Id);
            // event subscription not removed
            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.NotNull(execution);

            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            userTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskAfterMessage").First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterMessage", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());

            // now complete the IUser task with the message boundary event
            userTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            Assert.NotNull(userTask);

            taskService.Complete(userTask.Id);

            // event subscription removed
            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.IsNull(execution);

            userTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskAfterTask").First();
            Assert.NotNull(userTask);

            taskService.Complete(userTask.Id);

            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

            // 2nd. case: complete the IUser task cancels the message subscription

            runtimeService.StartProcessInstanceByKey("process");

            userTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task").First();
            Assert.NotNull(userTask);
            taskService.Complete(userTask.Id);

            execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName("messageName")*/.First();
            Assert.IsNull(execution);

            userTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskAfterTask").First();
            Assert.NotNull(userTask);
            Assert.AreEqual("taskAfterTask", userTask.TaskDefinitionKey);
            taskService.Complete(userTask.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingEventInCombinationWithReceiveTask()
        {
            // given
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when (1)
            runtimeService.CorrelateMessage("firstMessage");

            // then (1)
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            IExecution task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task1Execution).ParentId);

            // when (2)
            runtimeService.CorrelateMessage("secondMessage");

            // then (2)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task1Execution).ParentId);

            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task2").First();
            Assert.NotNull(task2);

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task2Execution).ParentId);

            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());

            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            AssertProcessEnded(ProcessInstanceId);

        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingEventInCombinationWithReceiveTaskInConcurrentSubprocess()
        {
            // given
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when (1)
            runtimeService.CorrelateMessage("firstMessage");

            // then (1)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            IExecution task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task1Execution).ParentId);


            // when (2)
            runtimeService.CorrelateMessage("secondMessage");

            // then (2)
            Assert.AreEqual(3, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());

            ITask afterFork = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="afterFork").First();
            taskService.Complete(afterFork.Id);

            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task2").First();
            Assert.NotNull(task2);

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task2").First();

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task2Execution).ParentId);

            taskService.Complete(task2.Id);
            taskService.Complete(task1.Id);

            AssertProcessEnded(ProcessInstanceId);

        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingEventInCombinationWithReceiveTaskInsideSubProcess()
        {
            // given
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("process");
            string ProcessInstanceId = instance.Id;

            // when (1)
            runtimeService.CorrelateMessage("firstMessage");

            // then (1)
            IActivityInstance activityInstance = runtimeService.GetActivityInstance(instance.Id);
            ActivityInstanceAssert.That(activityInstance).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                .BeginScope("subProcess").Activity("task1").BeginScope("innerSubProcess").Activity("receiveTask").Done());

            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            IExecution task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();

            Assert.IsFalse(ProcessInstanceId.Equals(((ExecutionEntity)task1Execution).ParentId));

            // when (2)
            runtimeService.CorrelateMessage("secondMessage");

            // then (2)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();

            Assert.IsFalse(ProcessInstanceId.Equals(((ExecutionEntity)task1Execution).ParentId));

            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task2").First();
            Assert.NotNull(task2);

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();

            Assert.IsFalse(ProcessInstanceId.Equals(((ExecutionEntity)task2Execution).ParentId));

            Assert.True(((ExecutionEntity)task1Execution).ParentId.Equals(((ExecutionEntity)task2Execution).ParentId));

            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());

            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingEventInCombinationWithUserTaskInsideSubProcess()
        {
            // given
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when (1)
            runtimeService.CorrelateMessage("firstMessage");

            // then (1)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            ITask innerTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="innerTask").First();
            Assert.NotNull(innerTask);

            ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);
            ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null)
                .Scope().Child(null).Scope().Child("task1").NoScope().Concurrent().Up().Child(null).NoScope().Concurrent().Child("innerTask").Scope().Done());

            // when (2)
            taskService.Complete(innerTask.Id);

            // then (2)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);


            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task2").First();
            Assert.NotNull(task2);

            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());

            executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);
            ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null)
                .Scope().Child(null).Scope().Child("task1").NoScope().Concurrent().Up().Child("task2").NoScope().Concurrent().Done());

            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingEventInCombinationWithUserTask()
        {
            // given
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when (1)
            runtimeService.CorrelateMessage("firstMessage");

            // then (1)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            ITask innerTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="innerTask").First();
            Assert.NotNull(innerTask);

            ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);
            ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null)
                .Scope().Child("task1").NoScope().Concurrent().Up().Child(null).NoScope().Concurrent().Child("innerTask").Scope().Done());

            // when (2)
            taskService.Complete(innerTask.Id);

            // then (2)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task2").First();
            Assert.NotNull(task2);

            executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);
            ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null)
                .Scope().Child("task1").NoScope().Concurrent().Up().Child("task2").NoScope().Concurrent().Done());

            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());

            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            AssertProcessEnded(ProcessInstanceId);
        }


        [Test]
        [Deployment]
        public virtual void testNonInterruptingWithUserTaskAndBoundaryEvent()
        {
            // given
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when (1)
            runtimeService.CorrelateMessage("firstMessage");

            // then (1)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            IExecution task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task1Execution).ParentId);

            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="innerTask").First();
            Assert.NotNull(task2);

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "innerTask").First();

            Assert.IsFalse(ProcessInstanceId.Equals(((ExecutionEntity)task2Execution).ParentId));

            // when (2)
            taskService.Complete(task2.Id);

            // then (2)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task1Execution).ParentId);

            task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task2").First();
            Assert.NotNull(task2);

            task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "tasks").First();

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task1Execution).ParentId);

            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());

            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testNestedEvents()
        {
            // given
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when (1)
            runtimeService.CorrelateMessage("firstMessage");

            // then (1)
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            ITask innerTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="innerTask").First();
            Assert.NotNull(innerTask);

            IExecution innerTaskExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "innerTask").First();

            Assert.IsFalse(ProcessInstanceId.Equals(((ExecutionEntity)innerTaskExecution).ParentId));

            // when (2)
            runtimeService.CorrelateMessage("secondMessage");

            // then (2)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            innerTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="innerTask").First();
            Assert.NotNull(innerTask);

            innerTaskExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "innerTask").First();

            Assert.IsFalse(ProcessInstanceId.Equals(((ExecutionEntity)innerTaskExecution).ParentId));

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            IExecution task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();
            Assert.NotNull(task1Execution);

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task1Execution).ParentId);

            // when (3)
            runtimeService.CorrelateMessage("thirdMessage");

            // then (3)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();
            Assert.NotNull(task1Execution);

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task1Execution).ParentId);

            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task2").First();
            Assert.NotNull(task2);

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task2").First();
            Assert.NotNull(task2Execution);

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task2Execution).ParentId);

            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());

            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            AssertProcessEnded(ProcessInstanceId);
        }


        [Test]
        [Deployment(new string[] { "resources/bpmn/event/message/MessageNonInterruptingBoundaryEventTest.testNestedEvents.bpmn20.xml" })]
        public virtual void testNestedEventsAnotherExecutionOrder()
        {
            // given
            string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when (1)
            runtimeService.CorrelateMessage("secondMessage");

            // then (1)
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());

            ITask task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            IExecution task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();
            Assert.NotNull(task1Execution);

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task1Execution).ParentId);

            // when (2)
            runtimeService.CorrelateMessage("firstMessage");

            // then (2)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            ITask innerTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="innerTask").First();
            Assert.NotNull(innerTask);

            IExecution innerTaskExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "innerTask").First();

            Assert.IsFalse(ProcessInstanceId.Equals(((ExecutionEntity)innerTaskExecution).ParentId));

            task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();
            Assert.NotNull(task1Execution);

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task1Execution).ParentId);

            // when (3)
            runtimeService.CorrelateMessage("thirdMessage");

            // then (3)
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

            task1 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task1").First();
            Assert.NotNull(task1);

            task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();
            Assert.NotNull(task1Execution);

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task1Execution).ParentId);

            ITask task2 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="task2").First();
            Assert.NotNull(task2);

            IExecution task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task2").First();
            Assert.NotNull(task2Execution);

            Assert.AreEqual(ProcessInstanceId, ((ExecutionEntity)task2Execution).ParentId);

            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery().Count());

            taskService.Complete(task1.Id);
            taskService.Complete(task2.Id);

            AssertProcessEnded(ProcessInstanceId);
        }

    }

}