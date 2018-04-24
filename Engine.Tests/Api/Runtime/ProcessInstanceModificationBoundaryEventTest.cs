using System.Linq;
using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ProcessInstanceModificationBoundaryEventTest : PluggableProcessEngineTestCase
    {
        protected internal const string INTERRUPTING_BOUNDARY_EVENT =
            "resources/api/runtime/ProcessInstanceModificationTest.interruptingBoundaryEvent.bpmn20.xml";

        protected internal const string NON_INTERRUPTING_BOUNDARY_EVENT =
            "resources/api/runtime/ProcessInstanceModificationTest.NonInterruptingBoundaryEvent.bpmn20.xml";

        protected internal const string INTERRUPTING_BOUNDARY_EVENT_INSIDE_SUBPROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.interruptingBoundaryEventInsideSubProcess.bpmn20.xml";

        protected internal const string NON_INTERRUPTING_BOUNDARY_EVENT_INSIDE_SUBPROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NonInterruptingBoundaryEventInsideSubProcess.bpmn20.xml";

        protected internal const string INTERRUPTING_BOUNDARY_EVENT_ON_SUBPROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.interruptingBoundaryEventOnSubProcess.bpmn20.xml";

        protected internal const string NON_INTERRUPTING_BOUNDARY_EVENT_ON_SUBPROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NonInterruptingBoundaryEventOnSubProcess.bpmn20.xml";

        protected internal const string INTERRUPTING_BOUNDARY_EVENT_WITH_PARALLEL_GATEWAY =
            "resources/api/runtime/ProcessInstanceModificationTest.interruptingBoundaryEventWithParallelGateway.bpmn20.xml";

        protected internal const string NON_INTERRUPTING_BOUNDARY_EVENT_WITH_PARALLEL_GATEWAY =
            "resources/api/runtime/ProcessInstanceModificationTest.NonInterruptingBoundaryEventWithParallelGateway.bpmn20.xml";

        protected internal const string INTERRUPTING_BOUNDARY_EVENT_WITH_PARALLEL_GATEWAY_INSIDE_SUB_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.interruptingBoundaryEventWithParallelGatewayInsideSubProcess.bpmn20.xml";

        protected internal const string NON_INTERRUPTING_BOUNDARY_EVENT_WITH_PARALLEL_GATEWAY_INSIDE_SUB_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NonInterruptingBoundaryEventWithParallelGatewayInsideSubProcess.bpmn20.xml";

        [Test]
        [Deployment( INTERRUPTING_BOUNDARY_EVENT)]
        public virtual void testTask1AndStartBeforeTaskAfterBoundaryEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("taskAfterBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .Activity("taskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("task1")
                    .Scope()
                    .Done());

            completeTasksInOrder("task1", "task2", "taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_BOUNDARY_EVENT)]
        public virtual void testTask1AndStartBeforeBoundaryEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("boundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("taskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("taskAfterBoundaryEvent")
                    .Scope()
                    .Done());

            completeTasksInOrder("taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_BOUNDARY_EVENT)]
        public virtual void testTask2AndStartBeforeTaskAfterBoundaryEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("taskAfterBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task2")
                    .Activity("taskAfterBoundaryEvent")
                    .Done());


            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("task2")
                    .Concurrent()
                    .NoScope()
                    .Done());

            completeTasksInOrder("task2", "taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_BOUNDARY_EVENT)]
        public virtual void testTask2AndStartBeforeBoundaryEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("boundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task2")
                    .Activity("taskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("task2")
                    .Concurrent()
                    .NoScope()
                    .Done());

            completeTasksInOrder("task2", "taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_BOUNDARY_EVENT)]
        public virtual void testTask1AndStartBeforeTaskAfterNonInterruptingBoundaryEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("taskAfterBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .Activity("taskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("task1")
                    .Scope()
                    .Done());

            completeTasksInOrder("task1", "taskAfterBoundaryEvent", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_BOUNDARY_EVENT)]
        public virtual void testTask1AndStartBeforeNonInterruptingBoundaryEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("boundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .Activity("taskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("task1")
                    .Scope()
                    .Done());

            completeTasksInOrder("task1", "taskAfterBoundaryEvent", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_BOUNDARY_EVENT)]
        public virtual void testTask2AndStartBeforeTaskAfterNonInterruptingBoundaryEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("taskAfterBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task2")
                    .Activity("taskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("task2")
                    .Concurrent()
                    .NoScope()
                    .Done());

            completeTasksInOrder("task2", "taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_BOUNDARY_EVENT)]
        public virtual void testTask2AndStartBeforeNonInterruptingBoundaryEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("boundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task2")
                    .Activity("taskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("task2")
                    .Concurrent()
                    .NoScope()
                    .Done());

            completeTasksInOrder("task2", "taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment( INTERRUPTING_BOUNDARY_EVENT_INSIDE_SUBPROCESS) ]
        public virtual void testTask1AndStartBeforeTaskAfterBoundaryEventInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("innerTaskAfterBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTask1")
                    .Activity("innerTaskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("innerTaskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask1")
                    .Scope()
                    .Done());

            completeTasksInOrder("innerTask1", "innerTaskAfterBoundaryEvent", "innerTask2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_BOUNDARY_EVENT_INSIDE_SUBPROCESS)]
        public virtual void testTask1AndStartBeforeBoundaryEventInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("innerBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTaskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("innerTaskAfterBoundaryEvent")
                    .Scope()
                    .Done());

            completeTasksInOrder("innerTaskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment( NON_INTERRUPTING_BOUNDARY_EVENT_INSIDE_SUBPROCESS) ]
        public virtual void testTask1AndStartBeforeTaskAfterNonInterruptingBoundaryEventInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("innerTaskAfterBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTask1")
                    .Activity("innerTaskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("innerTaskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask1")
                    .Scope()
                    .Done());

            completeTasksInOrder("innerTask1", "innerTaskAfterBoundaryEvent", "innerTask2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_BOUNDARY_EVENT_INSIDE_SUBPROCESS)]
        public virtual void testTask1AndStartBeforeNonInterruptingBoundaryEventInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("innerBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTask1")
                    .Activity("innerTaskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("innerTaskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask1")
                    .Scope()
                    .Done());

            completeTasksInOrder("innerTask1", "innerTask2", "innerTaskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_BOUNDARY_EVENT_INSIDE_SUBPROCESS)]
        public virtual void testTask2AndStartBeforeTaskAfterBoundaryEventInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("innerTaskAfterBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTask2")
                    .Activity("innerTaskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("innerTaskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("innerTask2")
                    .Concurrent()
                    .NoScope()
                    .Done());

            completeTasksInOrder("innerTask2", "innerTaskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_BOUNDARY_EVENT_INSIDE_SUBPROCESS)]
        public virtual void testTask2AndStartBeforeBoundaryEventInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("innerBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTask2")
                    .Activity("innerTaskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("innerTaskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("innerTask2")
                    .Concurrent()
                    .NoScope()
                    .Done());

            completeTasksInOrder("innerTask2", "innerTaskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_BOUNDARY_EVENT_INSIDE_SUBPROCESS)]
        public virtual void testTask2AndStartBeforeTaskAfterNonInterruptingBoundaryEventInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("innerTaskAfterBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTask2")
                    .Activity("innerTaskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("innerTaskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("innerTask2")
                    .Concurrent()
                    .NoScope()
                    .Done());

            completeTasksInOrder("innerTask2", "innerTaskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_BOUNDARY_EVENT_INSIDE_SUBPROCESS)]
        public virtual void testTask2AndStartBeforeNonInterruptingBoundaryEventInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("innerBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTask2")
                    .Activity("innerTaskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("innerTaskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("innerTask2")
                    .Concurrent()
                    .NoScope()
                    .Done());

            completeTasksInOrder("innerTask2", "innerTaskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_BOUNDARY_EVENT_ON_SUBPROCESS) ]
        public virtual void testStartBeforeTaskAfterBoundaryEventOnSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("taskAfterBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTask")
                    .EndScope()
                    .Activity("taskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("innerTask", "taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_BOUNDARY_EVENT_ON_SUBPROCESS)]
        public virtual void testStartBeforeBoundaryEventOnSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("boundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("taskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("taskAfterBoundaryEvent")
                    .Scope()
                    .Done());

            completeTasksInOrder("taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_BOUNDARY_EVENT_ON_SUBPROCESS)]
        public virtual void testStartBeforeTaskAfterNonInterruptingBoundaryEventOnSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("taskAfterBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTask")
                    .EndScope()
                    .Activity("taskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("innerTask", "taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_BOUNDARY_EVENT_ON_SUBPROCESS)]
        public virtual void testStartBeforeNonInterruptingBoundaryEventOnSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("taskAfterBoundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTask")
                    .EndScope()
                    .Activity("taskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask")
                    .Scope()
                    .Done());

            completeTasksInOrder("innerTask", "taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_BOUNDARY_EVENT_WITH_PARALLEL_GATEWAY)]
        public virtual void testStartBeforeInterruptingBoundaryEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("boundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .Activity("taskAfterBoundaryEvent")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Done());

            completeTasksInOrder("task1", "taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_BOUNDARY_EVENT_WITH_PARALLEL_GATEWAY) ]
        public virtual void testStartBeforeNonInterruptingBoundaryEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("boundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .Activity("task2")
                    .Activity("taskAfterBoundaryEvent")
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
                    .Child("task2")
                    .Scope()
                    .Up()
                    .Up()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Done());

            completeTasksInOrder("task1", "task2", "taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(INTERRUPTING_BOUNDARY_EVENT_WITH_PARALLEL_GATEWAY_INSIDE_SUB_PROCESS) ]
        public virtual void testStartBeforeInterruptingBoundaryEventInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("boundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("task1")
                    .Activity("taskAfterBoundaryEvent")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Done());

            completeTasksInOrder("task1", "taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NON_INTERRUPTING_BOUNDARY_EVENT_WITH_PARALLEL_GATEWAY_INSIDE_SUB_PROCESS)]
        public virtual void testStartBeforeNonInterruptingBoundaryEventInsideSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("boundaryEvent")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("task1")
                    .Activity("task2")
                    .Activity("taskAfterBoundaryEvent")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("task1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("task2")
                    .Scope()
                    .Up()
                    .Up()
                    .Child("taskAfterBoundaryEvent")
                    .Concurrent()
                    .NoScope()
                    .Done());

            completeTasksInOrder("task1", "task2", "taskAfterBoundaryEvent");
            AssertProcessEnded(ProcessInstanceId);
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