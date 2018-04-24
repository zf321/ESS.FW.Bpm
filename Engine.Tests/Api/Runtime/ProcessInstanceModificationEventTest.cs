using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ProcessInstanceModificationEventTest : PluggableProcessEngineTestCase
    {
        protected internal const string INTERMEDIATE_TIMER_CATCH_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.intermediateTimerCatch.bpmn20.xml";

        protected internal const string MESSAGE_START_EVENT_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.MessageStart.bpmn20.xml";

        protected internal const string TIMER_START_EVENT_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.TimerStart.bpmn20.xml";

        protected internal const string OneTaskProcess = "resources/api/runtime/oneTaskProcess.bpmn20.xml";

        protected internal const string TERMINATE_END_EVENT_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.TerminateEnd.bpmn20.xml";

        protected internal const string CANCEL_END_EVENT_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.CancelEnd.bpmn20.xml";

        [Test]
        [Deployment( INTERMEDIATE_TIMER_CATCH_PROCESS) ]
        public virtual void testStartBeforeIntermediateCatchEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("intermediateCatchEvent")
                .Execute();


            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task")
                    .Activity("intermediateCatchEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("intermediateCatchEvent")
                    .Scope()
                    .Done());

            var catchEventInstance = getChildInstanceForActivity(updatedTree, "intermediateCatchEvent");

            // and there is a timer job
            var job = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);
            Assert.AreEqual(catchEventInstance.ExecutionIds[0], job.ExecutionId);

            completeTasksInOrder("task");
            ExecuteAvailableJobs();
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment( MESSAGE_START_EVENT_PROCESS) ]
        public virtual void testStartBeforeMessageStartEvent()
        {
            runtimeService.CorrelateMessage("startMessage");
            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.NotNull(processInstance);

            var startEventSubscription = runtimeService.CreateEventSubscriptionQuery()
                .First();
            Assert.NotNull(startEventSubscription);

            var ProcessInstanceId = processInstance.Id;

            // when I start before the message start event
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("theStart")
                .Execute();

            // then there are two instances of "task"
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task")
                    .Activity("task")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("task")
                    .Concurrent()
                    .NoScope()
                    .Done());

            // and there is only the message start event subscription
            var subscription = runtimeService.CreateEventSubscriptionQuery()
                .First();
            Assert.NotNull(subscription);
            Assert.AreEqual(startEventSubscription.Id, subscription.Id);

            completeTasksInOrder("task", "task");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(TIMER_START_EVENT_PROCESS) ]
        public virtual void testStartBeforeTimerStartEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var startTimerJob = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(startTimerJob);

            // when I start before the timer start event
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("theStart")
                .Execute();

            // then there are two instances of "task"
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task")
                    .Activity("task")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("task")
                    .Concurrent()
                    .NoScope()
                    .Done());

            // and there is only one timer job
            var job = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);
            Assert.AreEqual(startTimerJob.Id, job.Id);

            completeTasksInOrder("task", "task");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(OneTaskProcess) ]
        public virtual void testStartBeforNoneStartEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            // when I start before the none start event
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("theStart")
                .Execute();

            // then there are two instances of "task"
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("theTask")
                    .Activity("theTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("theTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("theTask")
                    .Concurrent()
                    .NoScope()
                    .Done());

            // and the process can be ended as usual
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            foreach (var task in tasks)
                taskService.Complete(task.Id);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(OneTaskProcess)]
        public virtual void testStartBeforeNoneEndEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            // when I start before the none end event
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("theEnd")
                .Execute();

            // then there is no effect
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("theTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("theTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("theTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment( TERMINATE_END_EVENT_PROCESS)]
        public virtual void testStartBeforeTerminateEndEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            // when I start before the terminate end event
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("terminateEnd")
                .Execute();

            // then the process instance is terminated
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.IsNull(updatedTree);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(CANCEL_END_EVENT_PROCESS) ]
        public virtual void testStartBeforeCancelEndEventConcurrent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var txTask = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("txTask", txTask.TaskDefinitionKey);

            // when I start before the cancel end event
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("cancelEnd")
                .Execute();

            // then the subprocess instance is cancelled
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("afterCancellation")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("afterCancellation")
                    .Scope()
                    .Done());

            var afterCancellationTask = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(afterCancellationTask);
            Assert.IsFalse(txTask.Id.Equals(afterCancellationTask.Id));
            Assert.AreEqual("afterCancellation", afterCancellationTask.TaskDefinitionKey);
        }

        [Test]
        [Deployment(CANCEL_END_EVENT_PROCESS)]
        public virtual void testStartBeforeCancelEndEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            // complete the transaction subprocess once
            var txTask = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("txTask", txTask.TaskDefinitionKey);

            taskService.Complete(txTask.Id, ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("success", true));

            var afterSuccessTask = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("afterSuccess", afterSuccessTask.TaskDefinitionKey);

            // when I start before the cancel end event
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("cancelEnd")
                .Execute();

            // then a new subprocess instance is created and immediately cancelled
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("afterCancellation")
                    .Activity("afterSuccess")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("afterCancellation")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("afterSuccess")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("tx")
                    .Scope()
                    .EventScope()
                    .Done());

            // the compensation for the completed tx has not been triggered
            Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "undoTxTask")
                .Count());

            // complete the process
            var afterCancellationTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "afterCancellation")
                .First();
            Assert.NotNull(afterCancellationTask);

            taskService.Complete(afterCancellationTask.Id);
            taskService.Complete(afterSuccessTask.Id);

            AssertProcessEnded(ProcessInstanceId);
        }

        protected internal virtual IActivityInstance getChildInstanceForActivity(IActivityInstance activityInstance,
            string activityId)
        {
            foreach (var childInstance in activityInstance.ChildActivityInstances)
                if (childInstance.ActivityId.Equals(activityId))
                    return childInstance;

            return null;
        }

        protected internal virtual void completeTasksInOrder(params string[] taskNames)
        {
            foreach (var taskName in taskNames)
            {
                // complete any task with that name
                var tasks = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey==taskName)
                    /*.ListPage(0, 1)*/
                    .ToList();
                Assert.True(tasks.Count > 0, "task for activity " + taskName + " does not exist");
                taskService.Complete(tasks[0].Id);
            }
        }
    }
}