using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Bpmn.ExecutionListener;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    ///     Tests cancellation of four basic patterns of active activities in a scope:
    ///     <ul>
    ///         <li>
    ///             single, non-scope activity
    ///             <li>
    ///                 single, scope activity
    ///                 <li>
    ///                     two concurrent non-scope activities
    ///                     <li>two concurrent scope activities
    ///     </ul>
    /// </summary>
    [TestFixture]
    public class ProcessInstanceModificationCancellationTest : PluggableProcessEngineTestCase
    {
        // the four patterns as described above
        protected internal const string OneTaskProcess = "resources/api/runtime/oneTaskProcess.bpmn20.xml";

        protected internal const string ONE_SCOPE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.oneScopeTaskProcess.bpmn20.xml";

        protected internal const string CONCURRENT_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ParallelGateway.bpmn20.xml";

        protected internal const string CONCURRENT_SCOPE_TASKS_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.parallelGatewayScopeTasks.bpmn20.xml";

        // the four patterns nested in a subprocess and with an outer parallel task
        protected internal const string NESTED_PARALLEL_ONE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NestedParallelOneTaskProcess.bpmn20.xml";

        protected internal const string NESTED_PARALLEL_ONE_SCOPE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NestedParallelOneScopeTaskProcess.bpmn20.xml";

        protected internal const string NESTED_PARALLEL_CONCURRENT_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NestedParallelGateway.bpmn20.xml";

        protected internal const string NESTED_PARALLEL_CONCURRENT_SCOPE_TASKS_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.NestedParallelGatewayScopeTasks.bpmn20.xml";

        protected internal const string LISTENER_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.listenerProcess.bpmn20.xml";

        protected internal const string FAILING_OUTPUT_MAPPINGS_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.FailingOutputMappingProcess.bpmn20.xml";

        protected internal const string INTERRUPTING_EVENT_SUBPROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.interruptingEventSubProcess.bpmn20.xml";
        protected internal const string CALL_ACTIVITY_PROCESS = "resources/bpmn/callactivity/CallActivity.testCallSimpleSubProcess.bpmn20.xml";
        protected internal const string SIMPLE_SUBPROCESS = "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml";
        protected internal const string TWO_SUBPROCESSES = "resources/bpmn/callactivity/CallActivity.testTwoSubProcesses.bpmn20.xml";
        protected internal const string NESTED_CALL_ACTIVITY = "resources/bpmn/callactivity/CallActivity.testNestedCallActivity.bpmn20.xml";


        [Test]
        [Deployment(OneTaskProcess)]
        public virtual void testCancellationInOneTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "theTask"))
                .Execute();

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(OneTaskProcess)]
        public virtual void testCancelAllInOneTaskProcess()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            // two instance of theTask
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("theTask")
                .Execute();

            // when
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .CancelAllForActivity("theTask")
                .Execute();

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(OneTaskProcess)]
        public virtual void testCancellationAndCreationInOneTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "theTask"))
                .StartBeforeActivity("theTask")
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.AreEqual(tree.Id, updatedTree.Id);
            Assert.True(!getInstanceIdForActivity(tree, "theTask")
                .Equals(getInstanceIdForActivity(updatedTree, "theTask")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("theTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("theTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(OneTaskProcess)]
        public virtual void testCreationAndCancellationInOneTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("theTask")
                .CancelActivityInstance(getInstanceIdForActivity(tree, "theTask"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "theTask")
                .Equals(getInstanceIdForActivity(updatedTree, "theTask")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("theTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("theTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(ONE_SCOPE_TASK_PROCESS) ]
        public virtual void testCancellationInOneScopeTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "theTask"))
                .Execute();

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(ONE_SCOPE_TASK_PROCESS)]
        public virtual void testCancelAllInOneScopeTaskProcess()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            // two instances of theTask
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("theTask")
                .Execute();

            // then
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelAllForActivity("theTask")
                .Execute();

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(ONE_SCOPE_TASK_PROCESS)]
        public virtual void testCancellationAndCreationInOneScopeTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "theTask"))
                .StartBeforeActivity("theTask")
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "theTask")
                .Equals(getInstanceIdForActivity(updatedTree, "theTask")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("theTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("theTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(ONE_SCOPE_TASK_PROCESS)]
        public virtual void testCreationAndCancellationInOneScopeTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("theTask")
                .CancelActivityInstance(getInstanceIdForActivity(tree, "theTask"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "theTask")
                .Equals(getInstanceIdForActivity(updatedTree, "theTask")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("theTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("theTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(CONCURRENT_PROCESS) ]
        public virtual void testCancellationInConcurrentProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("parallelGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("task2")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(CONCURRENT_PROCESS)]
        public virtual void testCancelAllInConcurrentProcess()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("parallelGateway");
            var ProcessInstanceId = processInstance.Id;

            // two instances in task1
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("task1")
                .Execute();

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelAllForActivity("task1")
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("task2")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }


        [Test]
        [Deployment(CONCURRENT_PROCESS)]
        public virtual void testCancellationAndCreationInConcurrentProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("parallelGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .StartBeforeActivity("task1")
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "task1")
                .Equals(getInstanceIdForActivity(updatedTree, "task1")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .Activity("task2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .NoScope()
                    .Concurrent()
                    .Up()
                    .Child("task2")
                    .NoScope()
                    .Concurrent()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(CONCURRENT_PROCESS)]
        public virtual void testCreationAndCancellationInConcurrentProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("parallelGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task1")
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "task1")
                .Equals(getInstanceIdForActivity(updatedTree, "task1")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .Activity("task2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task1")
                    .NoScope()
                    .Concurrent()
                    .Up()
                    .Child("task2")
                    .NoScope()
                    .Concurrent()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(CONCURRENT_SCOPE_TASKS_PROCESS) ]
        public virtual void testCancellationInConcurrentScopeTasksProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("parallelGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "task1")
                .Equals(getInstanceIdForActivity(updatedTree, "task1")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task2")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(CONCURRENT_SCOPE_TASKS_PROCESS)]
        public virtual void testCancelAllInConcurrentScopeTasksProcess()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("parallelGateway");
            var ProcessInstanceId = processInstance.Id;

            // two instances of task1
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task1")
                .Execute();


            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelAllForActivity("task1")
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("task2")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }
        [Test]
        [Deployment(CONCURRENT_SCOPE_TASKS_PROCESS)]
        public virtual void testCancellationAndCreationInConcurrentScopeTasksProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("parallelGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .StartBeforeActivity("task1")
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "task1")
                .Equals(getInstanceIdForActivity(updatedTree, "task1")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .Activity("task2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .NoScope()
                    .Concurrent()
                    .Child("task1")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .NoScope()
                    .Concurrent()
                    .Child("task2")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(CONCURRENT_SCOPE_TASKS_PROCESS)]
        public virtual void testCreationAndCancellationInConcurrentScopeTasksProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("parallelGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task1")
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "task1")
                .Equals(getInstanceIdForActivity(updatedTree, "task1")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("task1")
                    .Activity("task2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .NoScope()
                    .Concurrent()
                    .Child("task1")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .NoScope()
                    .Concurrent()
                    .Child("task2")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment( NESTED_PARALLEL_ONE_TASK_PROCESS)]
        public virtual void testCancellationInNestedOneTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_ONE_TASK_PROCESS)]
        public virtual void testScopeCancellationInNestedOneTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "subProcess"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_ONE_TASK_PROCESS)]
        public virtual void testCancellationAndCreationInNestedOneTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask"))
                .StartBeforeActivity("innerTask")
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "innerTask")
                .Equals(getInstanceIdForActivity(updatedTree, "innerTask")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            foreach (var task in tasks)
                taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_ONE_TASK_PROCESS)]
        public virtual void testCreationAndCancellationInNestedOneTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("innerTask")
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "innerTask")
                .Equals(getInstanceIdForActivity(updatedTree, "innerTask")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            foreach (var task in tasks)
                taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_ONE_SCOPE_TASK_PROCESS) ]
        public virtual void testCancellationInNestedOneScopeTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneScopeTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_ONE_SCOPE_TASK_PROCESS)]
        public virtual void testScopeCancellationInNestedOneScopeTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneScopeTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "subProcess"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_ONE_SCOPE_TASK_PROCESS)]
        public virtual void testCancellationAndCreationInNestedOneScopeTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneScopeTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask"))
                .StartBeforeActivity("innerTask")
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "innerTask")
                .Equals(getInstanceIdForActivity(updatedTree, "innerTask")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("innerTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            foreach (var task in tasks)
                taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_ONE_SCOPE_TASK_PROCESS)]
        public virtual void testCreationAndCancellationInNestedOneScopeTaskProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedOneScopeTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("innerTask")
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "innerTask")
                .Equals(getInstanceIdForActivity(updatedTree, "innerTask")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("innerTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            foreach (var task in tasks)
                taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment( NESTED_PARALLEL_CONCURRENT_PROCESS)]
        public virtual void testCancellationInNestedConcurrentProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedParallelGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask2")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);
            foreach (var task in tasks)
                taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_CONCURRENT_PROCESS)]
        public virtual void testScopeCancellationInNestedConcurrentProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedParallelGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "subProcess"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_CONCURRENT_PROCESS)]
        public virtual void testCancellationAndCreationInNestedConcurrentProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedParallelGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1"))
                .StartBeforeActivity("innerTask1")
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "innerTask1")
                .Equals(getInstanceIdForActivity(updatedTree, "innerTask1")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask1")
                    .Activity("innerTask2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .NoScope()
                    .Concurrent()
                    .Up()
                    .Child(null)
                    .NoScope()
                    .Concurrent()
                    .Child(null)
                    .Scope()
                    .Child("innerTask1")
                    .NoScope()
                    .Concurrent()
                    .Up()
                    .Child("innerTask2")
                    .NoScope()
                    .Concurrent()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(3, tasks.Count);
            foreach (var task in tasks)
                taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_CONCURRENT_PROCESS)]
        public virtual void testCreationAndCancellationInNestedConcurrentProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedParallelGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("innerTask1")
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "innerTask1")
                .Equals(getInstanceIdForActivity(updatedTree, "innerTask1")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask1")
                    .Activity("innerTask2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .NoScope()
                    .Concurrent()
                    .Up()
                    .Child(null)
                    .NoScope()
                    .Concurrent()
                    .Child(null)
                    .Scope()
                    .Child("innerTask1")
                    .NoScope()
                    .Concurrent()
                    .Up()
                    .Child("innerTask2")
                    .NoScope()
                    .Concurrent()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(3, tasks.Count);
            foreach (var task in tasks)
                taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment( NESTED_PARALLEL_CONCURRENT_SCOPE_TASKS_PROCESS)]
        public virtual void testCancellationInNestedConcurrentScopeTasksProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedParallelGatewayScopeTasks");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("innerTask2")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);
            foreach (var task in tasks)
                taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_CONCURRENT_SCOPE_TASKS_PROCESS)]
        public virtual void testScopeCancellationInNestedConcurrentScopeTasksProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedParallelGatewayScopeTasks");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "subProcess"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree("outerTask")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_CONCURRENT_SCOPE_TASKS_PROCESS)]
        public virtual void testCancellationAndCreationInNestedConcurrentScopeTasksProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedParallelGatewayScopeTasks");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1"))
                .StartBeforeActivity("innerTask1")
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "innerTask1")
                .Equals(getInstanceIdForActivity(updatedTree, "innerTask1")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask1")
                    .Activity("innerTask2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask1")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask2")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(3, tasks.Count);

            foreach (var task in tasks)
                taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_CONCURRENT_SCOPE_TASKS_PROCESS)]
        public virtual void testCreationAndCancellationInNestedConcurrentScopeTasksProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedParallelGatewayScopeTasks");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("innerTask1")
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1"))
                .Execute();

            AssertProcessNotEnded(ProcessInstanceId);

            // Assert activity instance
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            Assert.True(!getInstanceIdForActivity(tree, "innerTask1")
                .Equals(getInstanceIdForActivity(updatedTree, "innerTask1")));

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask1")
                    .Activity("innerTask2")
                    .Done());

            // Assert executions
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("outerTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .NoScope()
                    .Concurrent()
                    .Child(null)
                    .Scope()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask1")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("innerTask2")
                    .Scope()
                    .Done());

            // Assert successful completion of process
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(3, tasks.Count);

            foreach (var task in tasks)
                taskService.Complete(task.Id);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(LISTENER_PROCESS)]
        public virtual void testEndListenerInvocation()
        {
            RecorderExecutionListener.Clear();

            var processInstance = runtimeService.StartProcessInstanceByKey("listenerProcess");
            //new Dictionary<string, ITypedValue>(){{"listener", new RecorderExecutionListener()));

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            // when one inner task is cancelled
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask1"))
                .Execute();

            Assert.AreEqual(1, RecorderExecutionListener.RecordedEvents.Count);
            var innerTask1EndEvent = RecorderExecutionListener.RecordedEvents[0];
            Assert.AreEqual(ExecutionListenerFields.EventNameEnd, innerTask1EndEvent.EventName);
            Assert.AreEqual("innerTask1", innerTask1EndEvent.ActivityId);
            Assert.AreEqual(getInstanceIdForActivity(tree, "innerTask1"), innerTask1EndEvent.ActivityInstanceId);

            // when the second inner task is cancelled
            RecorderExecutionListener.Clear();
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask2"))
                .Execute();

            Assert.AreEqual(2, RecorderExecutionListener.RecordedEvents.Count);
            var innerTask2EndEvent = RecorderExecutionListener.RecordedEvents[0];
            Assert.AreEqual(ExecutionListenerFields.EventNameEnd, innerTask2EndEvent.EventName);
            Assert.AreEqual("innerTask2", innerTask2EndEvent.ActivityId);
            Assert.AreEqual(getInstanceIdForActivity(tree, "innerTask2"), innerTask2EndEvent.ActivityInstanceId);

            var subProcessEndEvent = RecorderExecutionListener.RecordedEvents[1];
            Assert.AreEqual(ExecutionListenerFields.EventNameEnd, subProcessEndEvent.EventName);
            Assert.AreEqual("subProcess", subProcessEndEvent.ActivityId);
            Assert.AreEqual(getInstanceIdForActivity(tree, "subProcess"), subProcessEndEvent.ActivityInstanceId);

            // when the outer task is cancelled (and so the entire process)
            RecorderExecutionListener.Clear();
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "outerTask"))
                .Execute();

            Assert.AreEqual(2, RecorderExecutionListener.RecordedEvents.Count);
            var outerTaskEndEvent = RecorderExecutionListener.RecordedEvents[0];
            Assert.AreEqual(ExecutionListenerFields.EventNameEnd, outerTaskEndEvent.EventName);
            Assert.AreEqual("outerTask", outerTaskEndEvent.ActivityId);
            Assert.AreEqual(getInstanceIdForActivity(tree, "outerTask"), outerTaskEndEvent.ActivityInstanceId);

            var processEndEvent = RecorderExecutionListener.RecordedEvents[1];
            Assert.AreEqual(ExecutionListenerFields.EventNameEnd, processEndEvent.EventName);
            Assert.IsNull(processEndEvent.ActivityId);
            Assert.AreEqual(tree.Id, processEndEvent.ActivityInstanceId);

            RecorderExecutionListener.Clear();
        }

        /// <summary>
        ///     Tests the case that an output mapping exists that expects variables
        ///     that do not exist yet when the activities are cancelled
        /// </summary>
        [Test]
        [Deployment(FAILING_OUTPUT_MAPPINGS_PROCESS) ]
        public virtual void testSkipOutputMappingsOnCancellation()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("failingOutputMappingProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            // then executing the following cancellations should not Assert.Fail because
            // it skips the output mapping
            // cancel inner task
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask"))
                .Execute(false, true);

            // cancel outer task
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "outerTask"))
                .Execute(false, true);

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(INTERRUPTING_EVENT_SUBPROCESS) ]
        public virtual void testProcessInstanceEventSubscriptionsPreservedOnIntermediateCancellation()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");

            // event subscription for the event subprocess
            var subscription = runtimeService.CreateEventSubscriptionQuery()
                .First();
            Assert.NotNull(subscription);
            Assert.AreEqual(processInstance.Id, subscription.ProcessInstanceId);

            // when I execute cancellation and then start, such that the intermediate state of the process instance
            // has no activities
            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .StartBeforeActivity("task1")
                .Execute();

            // then the message event subscription remains (i.E. it is not deleted and later re-created)
            var updatedSubscription = runtimeService.CreateEventSubscriptionQuery()
                .First();
            Assert.NotNull(updatedSubscription);
            Assert.AreEqual(subscription.Id, updatedSubscription.Id);
            Assert.AreEqual(subscription.ProcessInstanceId, updatedSubscription.ProcessInstanceId);
        }

        [Test]
        [Deployment(OneTaskProcess)]
        public virtual void testProcessInstanceVariablesPreservedOnIntermediateCancellation()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("var", "value"));

            // when I execute cancellation and then start, such that the intermediate state of the process instance
            // has no activities
            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "theTask"))
                .StartBeforeActivity("theTask")
                .Execute();

            // then the process instance variables remain
            var variable = runtimeService.GetVariable(processInstance.Id, "var");
            Assert.NotNull(variable);
            Assert.AreEqual("value", variable);
        }

        public virtual string getInstanceIdForActivity(IActivityInstance activityInstance, string activityId)
        {
            var instance = getChildInstanceForActivity(activityInstance, activityId);
            if (instance != null)
                return instance.Id;
            return null;
        }

        public virtual IActivityInstance getChildInstanceForActivity(IActivityInstance activityInstance,
            string activityId)
        {
            if (activityId.Equals(activityInstance.ActivityId))
                return activityInstance;

            foreach (var childInstance in activityInstance.ChildActivityInstances)
            {
                var instance = getChildInstanceForActivity(childInstance, activityId);
                if (instance != null)
                    return instance;
            }

            return null;
        }


        /// <summary>
        /// Test case for checking cancellation of process instances in call activity subprocesses
        /// 
        /// Test should propagate upward and destroy all process instances
        /// 
        /// </summary>
        [Test][Deployment(new []{ SIMPLE_SUBPROCESS, CALL_ACTIVITY_PROCESS })]
        public virtual void TestCancellationInCallActivitySubProcess()
        {
            // given
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("callSimpleSubProcess");
            string processInstanceId = processInstance.Id;

            // one task in the subprocess should be active after starting the process instance
            var taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();

            // Completing the task continues the process which leads to calling the subprocess
            taskService.Complete(taskBeforeSubProcess.Id);
            ITask taskInSubProcess = taskQuery.First();


            IList<IProcessInstance> instanceList = runtimeService.CreateProcessInstanceQuery().ToList();
            Assert.NotNull(instanceList);
            Assert.Equals(2, instanceList.Count);

            IActivityInstance tree = runtimeService.GetActivityInstance(taskInSubProcess.ProcessInstanceId);
            // when
            runtimeService.CreateProcessInstanceModification(taskInSubProcess.ProcessInstanceId).CancelActivityInstance(getInstanceIdForActivity(tree, "task")).Execute();


            // then
            AssertProcessEnded(processInstanceId);

            // How many process Instances
            instanceList = runtimeService.CreateProcessInstanceQuery().ToList();
            Assert.NotNull(instanceList);
            Assert.Equals(0, instanceList.Count);
        }

        /// <summary>
        /// Test case for checking cancellation of process instances in call activity subprocesses
        /// 
        /// Test should propagate upward and destroy all process instances
        /// 
        /// </summary>
        [Test]
        [Deployment(new[] { SIMPLE_SUBPROCESS, CALL_ACTIVITY_PROCESS })]
        public virtual void TestCancellationAndRestartInCallActivitySubProcess()
        {
            // given
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("callSimpleSubProcess");
            string processInstanceId = processInstance.Id;

            // one task in the subprocess should be active after starting the process instance
            var taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();

            // Completing the task continues the process which leads to calling the subprocess
            taskService.Complete(taskBeforeSubProcess.Id);
            ITask taskInSubProcess = taskQuery.First();


            IList<IProcessInstance> instanceList = runtimeService.CreateProcessInstanceQuery().ToList();
            Assert.NotNull(instanceList);
            Assert.Equals(2, instanceList.Count);

            IActivityInstance tree = runtimeService.GetActivityInstance(taskInSubProcess.ProcessInstanceId);
            // when
            runtimeService.CreateProcessInstanceModification(taskInSubProcess.ProcessInstanceId).CancelActivityInstance(getInstanceIdForActivity(tree, "task")).StartBeforeActivity("task").Execute();


            // then


            // How many process Instances
            instanceList = runtimeService.CreateProcessInstanceQuery().ToList();
            Assert.Equals(2, instanceList.Count);
        }



        /// <summary>
        /// Test case for checking cancellation of process instances in call activity subprocesses
        /// 
        /// Test that upward cancellation respects other process instances
        /// 
        /// </summary>
        [Test]
        [Deployment(new[] { SIMPLE_SUBPROCESS, TWO_SUBPROCESSES })]
        public virtual void TestSingleCancellationWithTwoSubProcess()
        {
            // given
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("callTwoSubProcesses");
            IList<IProcessInstance> instanceList = runtimeService.CreateProcessInstanceQuery().ToList();
            Assert.NotNull(instanceList);
            Assert.Equals(3, instanceList.Count);

            IList<ITask> taskList = taskService.CreateTaskQuery().ToList();
            Assert.NotNull(taskList);
            Assert.Equals(2, taskList.Count);

            IList<string> activeActivityIds = runtimeService.GetActiveActivityIds(processInstance.ProcessInstanceId);
            Assert.NotNull(activeActivityIds);
            Assert.Equals(2, activeActivityIds.Count);


            IActivityInstance tree = runtimeService.GetActivityInstance(taskList[0].ProcessInstanceId);
            // when
            runtimeService.CreateProcessInstanceModification(taskList[0].ProcessInstanceId).CancelActivityInstance(getInstanceIdForActivity(tree, "task")).Execute();


            // then
            //assertProcessEnded(processInstanceId);

            // How many process Instances
            instanceList = runtimeService.CreateProcessInstanceQuery().ToList();
            Assert.NotNull(instanceList);
            Assert.Equals(2, instanceList.Count);

            // How man call activities
            activeActivityIds = runtimeService.GetActiveActivityIds(processInstance.ProcessInstanceId);
            Assert.NotNull(activeActivityIds);
            Assert.Equals(1, activeActivityIds.Count);
        }

        /// <summary>
        /// Test case for checking deletion of process instances in nested call activity subprocesses
        /// 
        /// Checking that nested call activities will propagate upward over multiple nested levels
        /// 
        /// </summary>
        [Deployment(new[] { SIMPLE_SUBPROCESS, NESTED_CALL_ACTIVITY, CALL_ACTIVITY_PROCESS })]
        public virtual void TestCancellationMultilevelProcessInstanceInCallActivity()
        {
            // given
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("nestedCallActivity");

            // one task in the subprocess should be active after starting the process instance
            var taskQuery = taskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();

            // Completing the task continues the process which leads to calling the subprocess
            taskService.Complete(taskBeforeSubProcess.Id);
            ITask taskInSubProcess = taskQuery.First();

            // Completing the task continues the sub process which leads to calling the deeper subprocess
            taskService.Complete(taskInSubProcess.Id);
            ITask taskInNestedSubProcess = taskQuery.First();

            IList<IProcessInstance> instanceList = runtimeService.CreateProcessInstanceQuery().ToList();
            Assert.NotNull(instanceList);
            Assert.Equals(3, instanceList.Count);

            IActivityInstance tree = runtimeService.GetActivityInstance(taskInNestedSubProcess.ProcessInstanceId);

            // when
            runtimeService.CreateProcessInstanceModification(taskInNestedSubProcess.ProcessInstanceId).CancelActivityInstance(getInstanceIdForActivity(tree, "task")).Execute();


            // then

            // How many process Instances
            instanceList = runtimeService.CreateProcessInstanceQuery().ToList();
            Assert.NotNull(instanceList);
            Assert.Equals(0, instanceList.Count);

        }

    }
}