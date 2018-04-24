using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ProcessInstanceModificationEventSubProcessTest : PluggableProcessEngineTestCase
    {
        protected internal const string INTERRUPTING_EVENT_SUBPROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.interruptingEventSubProcess.bpmn20.xml";

        protected internal const string NON_INTERRUPTING_EVENT_SUBPROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NonInterruptingEventSubProcess.bpmn20.xml";

        protected internal const string INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.interruptingEventSubProcessInsideSubProcess.bpmn20.xml";

        protected internal const string NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NonInterruptingEventSubProcessInsideSubProcess.bpmn20.xml";

        [Test]
        [Deployment( INTERRUPTING_EVENT_SUBPROCESS)]
        public virtual void testStartBeforeTaskInsideEventSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventSubProcessTask")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task1", "task2", "eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_EVENT_SUBPROCESS)]
        public virtual void testStartBeforeTaskInsideEventSubProcessAndCancelTaskOutsideEventSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .StartBeforeActivity("eventSubProcessTask")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_EVENT_SUBPROCESS)]
        public virtual void testStartBeforeStartEventInsideEventSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventProcessStart")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }
        [Test]
        [Deployment(INTERRUPTING_EVENT_SUBPROCESS)]
        public virtual void testStartBeforeEventSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventSubProcess")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment( NON_INTERRUPTING_EVENT_SUBPROCESS) ]
        public virtual void testStartBeforeTaskInsideNonInterruptingEventSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventSubProcessTask")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task1", "eventSubProcessTask", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_EVENT_SUBPROCESS)]
        public virtual void testStartBeforeTaskInsideNonInterruptingEventSubProcessAndCancelTaskOutsideEventSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .StartBeforeActivity("eventSubProcessTask")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_EVENT_SUBPROCESS)]
        public virtual void testStartBeforeStartEventInsideNonInterruptingEventSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventProcessStart")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task1", "task2", "eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_EVENT_SUBPROCESS)]
        public virtual void testStartBeforeNonInterruptingEventSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventSubProcess")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task1", "eventSubProcessTask", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment( INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
        public virtual void testStartBeforeTaskInsideEventSubProcessInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventSubProcessTask")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .BeginScope("subProcess")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task1", "eventSubProcessTask", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
        public virtual void testStartBeforeStartEventInsideEventSubProcessInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventProcessStart")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .BeginScope("subProcess")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("eventSubProcessTask", "task1", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
        public virtual void testStartBeforeEventSubProcessInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventSubProcess")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .BeginScope("subProcess")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task1", "eventSubProcessTask", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
        public virtual void testStartBeforeTaskInsideEventSubProcessInsideSubProcessTask2ShouldStay()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventSubProcessTask")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("task2")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("task2")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task2", "eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
        public virtual void testStartBeforeStartEventInsideEventSubProcessInsideSubProcessTask2ShouldBeCancelled()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventProcessStart")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
        public virtual void testStartBeforeEventSubProcessInsideSubProcessTask2ShouldBeCancelled()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventSubProcess")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS) ]
        public virtual void testStartBeforeTaskInsideNonInterruptingEventSubProcessInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventSubProcessTask")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .BeginScope("subProcess")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task1", "eventSubProcessTask", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
        public virtual void testStartBeforeStartEventInsideNonInterruptingEventSubProcessInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventProcessStart")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .BeginScope("subProcess")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task1", "task2", "eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
        public virtual void testStartBeforeNonInterruptingEventSubProcessInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventSubProcess")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .BeginScope("subProcess")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task1", "task2", "eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
        public virtual void testStartBeforeTaskInsideNonInterruptingEventSubProcessInsideSubProcessTask2ShouldStay()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventSubProcessTask")
                .Execute();


            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("task2")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("task2")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task2", "eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
        public virtual void testStartBeforeStartEventInsideNonInterruptingEventSubProcessInsideSubProcessTask2ShouldStay
            ()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventProcessStart")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("task2")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("task2")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task2", "eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_EVENT_SUBPROCESS_INSIDE_SUBPROCESS)]
        public virtual void testStartBeforeNonInterruptingEventSubProcessInsideSubProcessTask2ShouldStay()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("eventSubProcess")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("task2")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("task2")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("eventSubProcessTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("task2", "eventSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testTimerJobPreservationOnCancellationAndStart()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("timerEventSubProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            var timerJob = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(timerJob);

            // when the process instance is bare intermediately due to cancellation
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task"))
                .StartBeforeActivity("task")
                .Execute();

            // then it is still the same job

            var remainingTimerJob = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(remainingTimerJob);

            Assert.AreEqual(timerJob.Id, remainingTimerJob.Id);
            Assert.AreEqual(timerJob.Duedate, remainingTimerJob.Duedate);
        }

        protected internal virtual string getInstanceIdForActivity(IActivityInstance activityInstance, string activityId)
        {
            var instance = getChildInstanceForActivity(activityInstance, activityId);
            if (instance != null)
                return instance.Id;
            return null;
        }

        /// <summary>
        ///     Important that only the direct children are considered here. If you change this,
        ///     the test Assertions are not as tight anymore.
        /// </summary>
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