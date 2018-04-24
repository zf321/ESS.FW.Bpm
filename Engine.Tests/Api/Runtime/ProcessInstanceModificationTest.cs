using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Bpmn.ExecutionListener;
using Engine.Tests.Bpmn.TaskListener.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ProcessInstanceModificationTest : PluggableProcessEngineTestCase
    {
        protected internal const string PARALLEL_GATEWAY_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ParallelGateway.bpmn20.xml";

        protected internal const string EXCLUSIVE_GATEWAY_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ExclusiveGateway.bpmn20.xml";

        protected internal const string SUBPROCESS_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.Subprocess.bpmn20.xml";

        protected internal const string SUBPROCESS_LISTENER_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.SubprocessListeners.bpmn20.xml";

        protected internal const string SUBPROCESS_BOUNDARY_EVENTS_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.SubprocessBoundaryEvents.bpmn20.xml";

        protected internal const string ONE_SCOPE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.oneScopeTaskProcess.bpmn20.xml";

        protected internal const string TRANSITION_LISTENER_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.TransitionListeners.bpmn20.xml";

        protected internal const string TASK_LISTENER_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.TaskListeners.bpmn20.xml";

        protected internal const string IO_MAPPING_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ioMapping.bpmn20.xml";

        protected internal const string IO_MAPPING_ON_SUB_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ioMappingOnSubProcess.bpmn20.xml";

        protected internal const string IO_MAPPING_ON_SUB_PROCESS_AND_NESTED_SUB_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ioMappingOnSubProcessNested.bpmn20.xml";

        protected internal const string LISTENERS_ON_SUB_PROCESS_AND_NESTED_SUB_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.listenersOnSubProcessNested.bpmn20.xml";

        protected internal const string DOUBLE_NESTED_SUB_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.DoubleNestedSubprocess.bpmn20.xml";

        protected internal const string TRANSACTION_WITH_COMPENSATION_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.TestTransactionWithCompensation.bpmn20.xml";

        protected internal const string CALL_ACTIVITY_PARENT_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.TestCancelCallActivityParentProcess.bpmn";

        protected internal const string CALL_ACTIVITY_CHILD_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.TestCancelCallActivityChildProcess.bpmn";

        [Test]
        [Deployment(PARALLEL_GATEWAY_PROCESS)]
        public virtual void testCancellation()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("parallelGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("task2").Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree("task2").Scope().Done());

            // complete the process
            completeTasksInOrder("task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(PARALLEL_GATEWAY_PROCESS)]
        public virtual void testCancellationThatEndsProcessInstance()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("parallelGateway");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task2"))
                .Execute();

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(PARALLEL_GATEWAY_PROCESS)]
        public virtual void testCancellationWithWrongProcessInstanceId()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("parallelGateway");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            try
            {
                runtimeService.CreateProcessInstanceModification("foo")
                    .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                    .CancelActivityInstance(getInstanceIdForActivity(tree, "task2"))
                    .Execute();
                AssertProcessEnded(processInstance.Id);
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("ENGINE-13036"));
                Assert.That(e.Message, Does.Contain("Process instance '" + "foo" + "' cannot be modified"));
            }
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartBefore()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task2")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("task1").Activity("task2").Done());

            // ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child("task1").Concurrent().NoScope().Up().Child("task2").Concurrent().NoScope().Done());

            Assert.AreEqual(2, taskService.CreateTaskQuery()
                .Count());

            // complete the process
            completeTasksInOrder("task1", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartBeforeWithAncestorInstanceId()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task2", tree.Id)
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("task1").Activity("task2").Done());

            //ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child("task1").Concurrent().NoScope().Up().Child("task2").Concurrent().NoScope().Done());

            Assert.AreEqual(2, taskService.CreateTaskQuery()
                .Count());

            // complete the process
            completeTasksInOrder("task1", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(DOUBLE_NESTED_SUB_PROCESS) ]
        public virtual void testStartBeforeWithAncestorInstanceIdTwoScopesUp()
        {
            // given two instances of the outer subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("doubleNestedSubprocess");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("subProcess")
                .Execute();

            // when I start the inner subprocess task without explicit ancestor
            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartBeforeActivity("innerSubProcessTask")
                    .Execute();
                // then the command fails
                Assert.Fail("should not succeed because the ancestors are ambiguous");
            }
            catch (ProcessEngineException)
            {
                // happy path
            }

            // when I start the inner subprocess task with an explicit ancestor activity
            // instance id
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            var randomSubProcessInstance = getChildInstanceForActivity(updatedTree, "subProcess");

            // then the command suceeds
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("innerSubProcessTask", randomSubProcessInstance.Id)
                .Execute();

            // and the trees are correct
            updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("subProcess").Activity("subProcessTask").EndScope().BeginScope("subProcess").Activity("subProcessTask").BeginScope("innerSubProcess").Activity("innerSubProcessTask").Done());

            var innerSubProcessInstance = getChildInstanceForActivity(updatedTree, "innerSubProcess");
            Assert.AreEqual(randomSubProcessInstance.Id, innerSubProcessInstance.ParentActivityInstanceId);

            //ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child(null).Concurrent().NoScope().Child("subProcessTask").Scope().Up().Up().Child(null).Concurrent().NoScope().Child(null).Scope().Child("subProcessTask").Concurrent().NoScope().Up().Child(null).Concurrent().NoScope().Child("innerSubProcessTask").Scope().Done());

            Assert.AreEqual(3, taskService.CreateTaskQuery()
                .Count());

            // complete the process
            completeTasksInOrder("subProcessTask", "subProcessTask", "innerSubProcessTask", "innerSubProcessTask",
                "innerSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(DOUBLE_NESTED_SUB_PROCESS)]
        public virtual void testStartBeforeWithInvalidAncestorInstanceId()
        {
            // given two instances of the outer subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("doubleNestedSubprocess");
            var ProcessInstanceId = processInstance.Id;

            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartBeforeActivity("subProcess", "noValidActivityInstanceId")
                    .Execute();
                Assert.Fail();
            }
            catch (NotValidException e)
            {
                // happy path
                AssertTextPresent(
                    "Cannot perform instruction: " +
                    "Start before activity 'subProcess' with ancestor activity instance 'noValidActivityInstanceId'; " +
                    "Ancestor activity instance 'noValidActivityInstanceId' does not exist", e.Message);
            }

            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartBeforeActivity("subProcess", null)
                    .Execute();
                Assert.Fail();
            }
            catch (NotValidException e)
            {
                // happy path
                AssertTextPresent("ancestorActivityInstanceId is null", e.Message);
            }

            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);
            var subProcessTaskId = getInstanceIdForActivity(tree, "subProcessTask");

            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartBeforeActivity("subProcess", subProcessTaskId)
                    .Execute();
                Assert.Fail("should not succeed because subProcessTask is a child of subProcess");
            }
            catch (NotValidException e)
            {
                // happy path
                AssertTextPresent(
                    "Cannot perform instruction: " +
                    "Start before activity 'subProcess' with ancestor activity instance '" + subProcessTaskId + "'; " +
                    "Scope execution for '" + subProcessTaskId +
                    "' cannot be found in parent hierarchy of flow element 'subProcess'", e.Message);
            }
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartBeforeNonExistingActivity()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");

            try
            {
                // when
                runtimeService.CreateProcessInstanceModification(instance.Id)
                    .StartBeforeActivity("someNonExistingActivity")
                    .Execute();
                Assert.Fail("should not succeed");
            }
            catch (NotValidException e)
            {
                // then
                AssertTextPresentIgnoreCase("element 'someNonExistingActivity' does not exist in process ", e.Message);
            }
        }

        /// <summary>
        ///     CAM-3718
        /// </summary>
        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testEndProcessInstanceIntermediately()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .StartAfterActivity("task1")
                .StartBeforeActivity("task1")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("task1").Done());

            //ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree("task1").Scope().Done());

            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());

            // complete the process
            completeTasksInOrder("task1");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartTransition()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartTransition("flow4")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("task1").Activity("task2").Done());

            //ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child("task1").Concurrent().NoScope().Up().Child("task2").Concurrent().NoScope().Done());

            Assert.AreEqual(2, taskService.CreateTaskQuery()
                .Count());

            // complete the process
            completeTasksInOrder("task1", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartTransitionWithAncestorInstanceId()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartTransition("flow4", tree.Id)
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("task1").Activity("task2").Done());

            //ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child("task1").Concurrent().NoScope().Up().Child("task2").Concurrent().NoScope().Done());

            Assert.AreEqual(2, taskService.CreateTaskQuery()
                .Count());

            // complete the process
            completeTasksInOrder("task1", "task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(DOUBLE_NESTED_SUB_PROCESS)]
        public virtual void testStartTransitionWithAncestorInstanceIdTwoScopesUp()
        {
            // given two instances of the outer subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("doubleNestedSubprocess");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("subProcess")
                .Execute();

            // when I start the inner subprocess task without explicit ancestor
            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartTransition("flow5")
                    .Execute();
                // then the command fails
                Assert.Fail("should not succeed because the ancestors are ambiguous");
            }
            catch (ProcessEngineException)
            {
                // happy path
            }

            // when I start the inner subprocess task with an explicit ancestor activity
            // instance id
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            var randomSubProcessInstance = getChildInstanceForActivity(updatedTree, "subProcess");

            // then the command suceeds
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartTransition("flow5", randomSubProcessInstance.Id)
                .Execute();

            // and the trees are correct
            updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("subProcess").Activity("subProcessTask").EndScope().BeginScope("subProcess").Activity("subProcessTask").BeginScope("innerSubProcess").Activity("innerSubProcessTask").Done());

            var innerSubProcessInstance = getChildInstanceForActivity(updatedTree, "innerSubProcess");
            Assert.AreEqual(randomSubProcessInstance.Id, innerSubProcessInstance.ParentActivityInstanceId);

            //ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child(null).Concurrent().NoScope().Child("subProcessTask").Scope().Up().Up().Child(null).Concurrent().NoScope().Child(null).Scope().Child("subProcessTask").Concurrent().NoScope().Up().Child(null).Concurrent().NoScope().Child("innerSubProcessTask").Scope().Done());

            Assert.AreEqual(3, taskService.CreateTaskQuery()
                .Count());

            // complete the process
            completeTasksInOrder("subProcessTask", "subProcessTask", "innerSubProcessTask", "innerSubProcessTask",
                "innerSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(DOUBLE_NESTED_SUB_PROCESS)]
        public virtual void testStartTransitionWithInvalidAncestorInstanceId()
        {
            // given two instances of the outer subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("doubleNestedSubprocess");
            var ProcessInstanceId = processInstance.Id;

            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartTransition("flow5", "noValidActivityInstanceId")
                    .Execute();
                Assert.Fail();
            }
            catch (NotValidException e)
            {
                // happy path
                AssertTextPresent(
                    "Cannot perform instruction: " +
                    "Start transition 'flow5' with ancestor activity instance 'noValidActivityInstanceId'; " +
                    "Ancestor activity instance 'noValidActivityInstanceId' does not exist", e.Message);
            }

            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartTransition("flow5", null)
                    .Execute();
                Assert.Fail();
            }
            catch (NotValidException e)
            {
                // happy path
                AssertTextPresent("ancestorActivityInstanceId is null", e.Message);
            }

            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);
            var subProcessTaskId = getInstanceIdForActivity(tree, "subProcessTask");

            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartTransition("flow5", subProcessTaskId)
                    .Execute();
                Assert.Fail("should not succeed because subProcessTask is a child of subProcess");
            }
            catch (NotValidException e)
            {
                // happy path
                AssertTextPresent(
                    "Cannot perform instruction: " + "Start transition 'flow5' with ancestor activity instance '" +
                    subProcessTaskId + "'; " + "Scope execution for '" + subProcessTaskId +
                    "' cannot be found in parent hierarchy of flow element 'flow5'", e.Message);
            }
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartTransitionCase2()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartTransition("flow2")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("task1").Activity("task1").Done());

            //ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child("task1").Concurrent().NoScope().Up().Child("task1").Concurrent().NoScope().Done());

            Assert.AreEqual(2, taskService.CreateTaskQuery()
                .Count());

            // complete the process
            completeTasksInOrder("task1", "task1");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartTransitionInvalidTransitionId()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            try
            {
                runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                    .StartTransition("invalidFlowId")
                    .Execute();

                Assert.Fail("should not suceed");
            }
            catch (ProcessEngineException e)
            {
                // happy path
                AssertTextPresent(
                    "Cannot perform instruction: " + "Start transition 'invalidFlowId'; " +
                    "Element 'invalidFlowId' does not exist in process '" + processInstance.ProcessDefinitionId + "'",
                    e.Message);
            }
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartAfter()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartAfterActivity("theStart")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("task1").Activity("task1").Done());

            //ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child("task1").Concurrent().NoScope().Up().Child("task1").Concurrent().NoScope().Done());

            Assert.AreEqual(2, taskService.CreateTaskQuery()
                .Count());

            // complete the process
            completeTasksInOrder("task1", "task1");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartAfterWithAncestorInstanceId()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartAfterActivity("theStart", tree.Id)
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("task1").Activity("task1").Done());

            //ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child("task1").Concurrent().NoScope().Up().Child("task1").Concurrent().NoScope().Done());

            Assert.AreEqual(2, taskService.CreateTaskQuery()
                .Count());

            // complete the process
            completeTasksInOrder("task1", "task1");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(DOUBLE_NESTED_SUB_PROCESS)]
        public virtual void testStartAfterWithAncestorInstanceIdTwoScopesUp()
        {
            // given two instances of the outer subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("doubleNestedSubprocess");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("subProcess")
                .Execute();

            // when I start the inner subprocess task without explicit ancestor
            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartAfterActivity("innerSubProcessStart")
                    .Execute();
                // then the command fails
                Assert.Fail("should not succeed because the ancestors are ambiguous");
            }
            catch (ProcessEngineException)
            {
                // happy path
            }

            // when I start the inner subprocess task with an explicit ancestor activity
            // instance id
            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            var randomSubProcessInstance = getChildInstanceForActivity(updatedTree, "subProcess");

            // then the command suceeds
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartAfterActivity("innerSubProcessStart", randomSubProcessInstance.Id)
                .Execute();

            // and the trees are correct
            updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("subProcess").Activity("subProcessTask").EndScope().BeginScope("subProcess").Activity("subProcessTask").BeginScope("innerSubProcess").Activity("innerSubProcessTask").Done());

            var innerSubProcessInstance = getChildInstanceForActivity(updatedTree, "innerSubProcess");
            Assert.AreEqual(randomSubProcessInstance.Id, innerSubProcessInstance.ParentActivityInstanceId);

            //ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child(null).Concurrent().NoScope().Child("subProcessTask").Scope().Up().Up().Child(null).Concurrent().NoScope().Child(null).Scope().Child("subProcessTask").Concurrent().NoScope().Up().Child(null).Concurrent().NoScope().Child("innerSubProcessTask").Scope().Done());

            Assert.AreEqual(3, taskService.CreateTaskQuery()
                .Count());

            // complete the process
            completeTasksInOrder("subProcessTask", "subProcessTask", "innerSubProcessTask", "innerSubProcessTask",
                "innerSubProcessTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(DOUBLE_NESTED_SUB_PROCESS)]
        public virtual void testStartAfterWithInvalidAncestorInstanceId()
        {
            // given two instances of the outer subprocess
            var processInstance = runtimeService.StartProcessInstanceByKey("doubleNestedSubprocess");
            var ProcessInstanceId = processInstance.Id;

            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartAfterActivity("innerSubProcessStart", "noValidActivityInstanceId")
                    .Execute();
                Assert.Fail();
            }
            catch (NotValidException e)
            {
                // happy path
                AssertTextPresent(
                    "Cannot perform instruction: " +
                    "Start after activity 'innerSubProcessStart' with ancestor activity instance 'noValidActivityInstanceId'; " +
                    "Ancestor activity instance 'noValidActivityInstanceId' does not exist", e.Message);
            }

            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartAfterActivity("innerSubProcessStart", null)
                    .Execute();
                Assert.Fail();
            }
            catch (NotValidException e)
            {
                // happy path
                AssertTextPresent("ancestorActivityInstanceId is null", e.Message);
            }

            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);
            var subProcessTaskId = getInstanceIdForActivity(tree, "subProcessTask");

            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartAfterActivity("innerSubProcessStart", subProcessTaskId)
                    .Execute();
                Assert.Fail("should not succeed because subProcessTask is a child of subProcess");
            }
            catch (NotValidException e)
            {
                // happy path
                AssertTextPresent(
                    "Cannot perform instruction: " +
                    "Start after activity 'innerSubProcessStart' with ancestor activity instance '" + subProcessTaskId +
                    "'; " + "Scope execution for '" + subProcessTaskId +
                    "' cannot be found in parent hierarchy of flow element 'flow5'", e.Message);
            }
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartAfterActivityAmbiguousTransitions()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            try
            {
                runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                    .StartAfterActivity("fork")
                    .Execute();

                Assert.Fail("should not suceed since 'fork' has more than one outgoing sequence flow");
            }
            catch (ProcessEngineException e)
            {
                // happy path
                AssertTextPresent("activity has more than one outgoing sequence flow", e.Message);
            }
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartAfterActivityNoOutgoingTransitions()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            try
            {
                runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                    .StartAfterActivity("theEnd")
                    .Execute();

                Assert.Fail("should not suceed since 'theEnd' has no outgoing sequence flow");
            }
            catch (ProcessEngineException e)
            {
                // happy path
                AssertTextPresent("activity has no outgoing sequence flow to take", e.Message);
            }
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartAfterNonExistingActivity()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");

            try
            {
                // when
                runtimeService.CreateProcessInstanceModification(instance.Id)
                    .StartAfterActivity("someNonExistingActivity")
                    .Execute();
                Assert.Fail("should not succeed");
            }
            catch (NotValidException e)
            {
                // then
                AssertTextPresentIgnoreCase(
                    "Cannot perform instruction: " + "Start after activity 'someNonExistingActivity'; " +
                    "Activity 'someNonExistingActivity' does not exist: activity is null", e.Message);
            }
        }

        [Test]
        [Deployment(ONE_SCOPE_TASK_PROCESS) ]
        public virtual void testScopeTaskStartBefore()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("theTask")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("theTask").Activity("theTask").Done());

            //ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child(null).Concurrent().NoScope().Child("theTask").Scope().Up().Up().Child(null).Concurrent().NoScope().Child("theTask").Scope().Done());

            Assert.AreEqual(2, taskService.CreateTaskQuery()
                .Count());
            completeTasksInOrder("theTask", "theTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(ONE_SCOPE_TASK_PROCESS)]
        public virtual void testScopeTaskStartAfter()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var ProcessInstanceId = processInstance.Id;

            // when starting after the task, essentially nothing changes in the process
            // instance
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartAfterActivity("theTask")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("theTask").Done());

            //ExecutionTree executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child("theTask").Scope().Done());

            // when starting after the start event, regular concurrency happens
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartAfterActivity("theStart")
                .Execute();

            updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("theTask").Activity("theTask").Done());

            //executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child(null).Concurrent().NoScope().Child("theTask").Scope().Up().Up().Child(null).Concurrent().NoScope().Child("theTask").Scope().Done());

            completeTasksInOrder("theTask", "theTask");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(SUBPROCESS_BOUNDARY_EVENTS_PROCESS)]
        public virtual void testStartBeforeEventSubscription()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("subprocess");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("innerTask")
                .Execute();

            // then two timer jobs should have been created
            Assert.AreEqual(2, managementService.CreateJobQuery()
                .Count());
            var innerJob = managementService.CreateJobQuery(c=> c.ActivityId =="innerTimer")
                .First();
            Assert.NotNull(innerJob);
            Assert.AreEqual(runtimeService.CreateExecutionQuery(c=>c.ActivityId == "innerTask")
                .First()
                .Id, innerJob.ExecutionId);

            var outerJob = managementService.CreateJobQuery(c=> c.ActivityId =="outerTimer")
                .First();
            Assert.NotNull(outerJob);

            // when executing the jobs
            managementService.ExecuteJob(innerJob.Id);

            var innerBoundaryTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "innerAfterBoundaryTask")
                .First();
            Assert.NotNull(innerBoundaryTask);

            managementService.ExecuteJob(outerJob.Id);

            var outerBoundaryTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "outerAfterBoundaryTask")
                .First();
            Assert.NotNull(outerBoundaryTask);
        }

        [Test]
        [Deployment(SUBPROCESS_LISTENER_PROCESS) ]
        public virtual void testActivityExecutionListenerInvocation()
        {
            RecorderExecutionListener.Clear();

            var processInstance = runtimeService.StartProcessInstanceByKey("subprocess",
                new Dictionary<string, object> {{"listener", new RecorderExecutionListener()}});

            var ProcessInstanceId = processInstance.Id;

            Assert.True(RecorderExecutionListener.RecordedEvents.Count == 0);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("innerTask")
                .Execute();

            // Assert activity instance tree
            var activityInstanceTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(activityInstanceTree);
            Assert.AreEqual(ProcessInstanceId, activityInstanceTree.ProcessInstanceId);

            //Assert.That(activityInstanceTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("outerTask").BeginScope("subProcess").Activity("innerTask").Done());

            // Assert listener invocations
            var recordedEvents = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(2, recordedEvents.Count);

            var subprocessInstance = getChildInstanceForActivity(activityInstanceTree, "subProcess");
            var innerTaskInstance = getChildInstanceForActivity(subprocessInstance, "innerTask");

            var firstEvent = recordedEvents[0];
            var secondEvent = recordedEvents[1];

            Assert.AreEqual("subProcess", firstEvent.ActivityId);
            Assert.AreEqual(subprocessInstance.Id, firstEvent.ActivityInstanceId);
            Assert.AreEqual(ExecutionListenerFields.EventNameStart, secondEvent.EventName);

            Assert.AreEqual("innerTask", secondEvent.ActivityId);
            Assert.AreEqual(innerTaskInstance.Id, secondEvent.ActivityInstanceId);
            Assert.AreEqual(ExecutionListenerFields.EventNameStart, secondEvent.EventName);

            RecorderExecutionListener.Clear();

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(innerTaskInstance.Id)
                .Execute();

            Assert.AreEqual(2, RecorderExecutionListener.RecordedEvents.Count);
        }

        [Test]
        [Deployment(SUBPROCESS_LISTENER_PROCESS)]
        public virtual void testSkipListenerInvocation()
        {
            RecorderExecutionListener.Clear();

            var processInstance = runtimeService.StartProcessInstanceByKey("subprocess",
                new Dictionary<string, object> {{"listener", new RecorderExecutionListener()}});

            var ProcessInstanceId = processInstance.Id;

            Assert.True(RecorderExecutionListener.RecordedEvents.Count == 0);

            // when I start an activity with "skip listeners" setting
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("innerTask")
                .Execute(true, false);

            // then no listeners are invoked
            Assert.True(RecorderExecutionListener.RecordedEvents.Count == 0);

            // when I cancel an activity with "skip listeners" setting
            var activityInstanceTree = runtimeService.GetActivityInstance(ProcessInstanceId);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getChildInstanceForActivity(activityInstanceTree, "innerTask")
                    .Id)
                .Execute(true, false);

            // then no listeners are invoked
            Assert.True(RecorderExecutionListener.RecordedEvents.Count == 0);

            // when I cancel an activity that ends the process instance
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getChildInstanceForActivity(activityInstanceTree, "outerTask")
                    .Id)
                .Execute(true, false);

            // then no listeners are invoked
            Assert.True(RecorderExecutionListener.RecordedEvents.Count == 0);
        }

        [Test]
        [Deployment(TASK_LISTENER_PROCESS) ]
        public virtual void testSkipTaskListenerInvocation()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("taskListenerProcess",
                new Dictionary<string, object> {{"listener", new RecorderTaskListener()}});

            var ProcessInstanceId = processInstance.Id;

            RecorderTaskListener.clear();

            // when I start an activity with "skip listeners" setting
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("task")
                .Execute(true, false);

            // then no listeners are invoked
            Assert.True(RecorderTaskListener.RecordedEvents.Count == 0);

            // when I cancel an activity with "skip listeners" setting
            var activityInstanceTree = runtimeService.GetActivityInstance(ProcessInstanceId);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getChildInstanceForActivity(activityInstanceTree, "task")
                    .Id)
                .Execute(true, false);

            // then no listeners are invoked
            Assert.True(RecorderTaskListener.RecordedEvents.Count == 0);
        }

        [Test]
        [Deployment(IO_MAPPING_PROCESS) ]
        public virtual void testSkipIoMappings()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("ioMappingProcess");

            // when I start task2
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task2")
                .Execute(false, true);

            // then the input mapping should not have executed
            var task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task2")
                .First();
            Assert.NotNull(task2Execution);

            Assert.IsNull(runtimeService.GetVariable(task2Execution.Id, "inputMappingExecuted"));

            // when I cancel task2
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelAllForActivity("task2")
                .Execute(false, true);

            // then the output mapping should not have executed
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "outputMappingExecuted"));
        }

        [Test]
        [Deployment(IO_MAPPING_ON_SUB_PROCESS) ]
        public virtual void testSkipIoMappingsOnSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("boundaryEvent")
                .Execute(false, true);

            // then the output mapping should not have executed
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "outputMappingExecuted"));
        }

        /// <summary>
        ///     should also skip io mappings that are defined on already instantiated
        ///     ancestor scopes and that may be executed due to the ancestor scope
        ///     completing within the modification command.
        /// </summary>
        [Test]
        [Deployment(IO_MAPPING_ON_SUB_PROCESS_AND_NESTED_SUB_PROCESS)]
        public virtual void testSkipIoMappingsOnSubProcessNested()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("boundaryEvent")
                .Execute(false, true);

            // then the output mapping should not have executed
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "outputMappingExecuted"));
        }

        [Test]
        [Deployment(LISTENERS_ON_SUB_PROCESS_AND_NESTED_SUB_PROCESS) ]
        public virtual void testSkipListenersOnSubProcessNested()
        {
            RecorderExecutionListener.Clear();

            var processInstance = runtimeService.StartProcessInstanceByKey("process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("listener", new RecorderExecutionListener()));

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("boundaryEvent")
                .Execute(true, false);

            AssertProcessEnded(processInstance.Id);

            // then the output mapping should not have executed
            Assert.True(RecorderExecutionListener.RecordedEvents.Count == 0);
        }

        [Test]
        [Deployment(TRANSITION_LISTENER_PROCESS) ]
        public virtual void testStartTransitionListenerInvocation()
        {
            RecorderExecutionListener.Clear();

            var instance = runtimeService.StartProcessInstanceByKey("transitionListenerProcess",
                ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("listener", new RecorderExecutionListener()));

            runtimeService.CreateProcessInstanceModification(instance.Id)
                .StartTransition("flow2")
                .Execute();

            // transition listener should have been invoked
            var events = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(1, events.Count);

            var @event = events[0];
            Assert.AreEqual("flow2", @event.TransitionId);

            RecorderExecutionListener.Clear();

            var updatedTree = runtimeService.GetActivityInstance(instance.Id);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(instance.Id, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId).Activity("task1").Activity("task2").Done());

            var executionTree = ExecutionTree.ForExecution(instance.Id, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child("task1").Concurrent().NoScope().Up().Child("task2").Concurrent().NoScope().Done());

            completeTasksInOrder("task1", "task2", "task2");
            AssertProcessEnded(instance.Id);
        }

        [Test]
        [Deployment(TRANSITION_LISTENER_PROCESS)]
        public virtual void testStartAfterActivityListenerInvocation()
        {
            RecorderExecutionListener.Clear();

            var instance = runtimeService.StartProcessInstanceByKey("transitionListenerProcess",
                ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("listener", new RecorderExecutionListener()));

            runtimeService.CreateProcessInstanceModification(instance.Id)
                .StartTransition("flow2")
                .Execute();

            // transition listener should have been invoked
            var events = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(1, events.Count);

            var @event = events[0];
            Assert.AreEqual("flow2", @event.TransitionId);

            RecorderExecutionListener.Clear();

            var updatedTree = runtimeService.GetActivityInstance(instance.Id);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(instance.Id, updatedTree.ProcessInstanceId);

            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId).Activity("task1").Activity("task2").Done());

            var executionTree = ExecutionTree.ForExecution(instance.Id, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree(null).Scope().Child("task1").Concurrent().NoScope().Up().Child("task2").Concurrent().NoScope().Done());

            completeTasksInOrder("task1", "task2", "task2");
            AssertProcessEnded(instance.Id);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartBeforeWithVariables()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task2")
                .SetVariable("procInstVar", "procInstValue")
                .SetVariableLocal("localVar", "localValue")
                //.SetVariables(Variable.Variables.CreateVariables()
                //    .PutValue("procInstMapVar", "procInstMapValue"))
                //.SetVariablesLocal(Variable.Variables.CreateVariables()
                //    .PutValue("localMapVar", "localMapValue"))
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);
            Assert.NotNull(updatedTree);
            //ActivityInstanceAssert.That(updatedTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("task1").Activity("task2").Done());

            var task2Instance = getChildInstanceForActivity(updatedTree, "task2");
            Assert.NotNull(task2Instance);
            Assert.AreEqual(1, task2Instance.ExecutionIds.Length);
            var task2ExecutionId = task2Instance.ExecutionIds[0];

            Assert.AreEqual(4, runtimeService.CreateVariableInstanceQuery()
                .Count());
            Assert.AreEqual("procInstValue", runtimeService.GetVariableLocal(processInstance.Id, "procInstVar"));
            Assert.AreEqual("localValue", runtimeService.GetVariableLocal(task2ExecutionId, "localVar"));
            Assert.AreEqual("procInstMapValue", runtimeService.GetVariableLocal(processInstance.Id, "procInstMapVar"));
            Assert.AreEqual("localMapValue", runtimeService.GetVariableLocal(task2ExecutionId, "localMapVar"));

            completeTasksInOrder("task1", "task2");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testCancellationAndStartBefore()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");
            var ProcessInstanceId = processInstance.Id;

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "task1"))
                .StartBeforeActivity("task2")
                .Execute();

            var activityInstanceTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(activityInstanceTree);
            Assert.AreEqual(ProcessInstanceId, activityInstanceTree.ProcessInstanceId);

            //Assert.That(activityInstanceTree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("task2").Done());

            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            //ExecutionAssert.That(executionTree).Matches(ExecutionAssert.DescribeExecutionTree("task2").Scope().Done());

            completeTasksInOrder("task2");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testCompensationRemovalOnCancellation()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensationProcess");

            var taskExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "innerTask")
                .First();
            var task = taskService.CreateTaskQuery(c=>c.ExecutionId ==taskExecution.Id)
                .First();
            Assert.NotNull(task);

            taskService.Complete(task.Id);
            // there should be a compensation event subscription for innerTask now
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery()
                .Count());

            // when innerTask2 is cancelled
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask2"))
                .Execute();

            // then the innerTask compensation should be removed
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery()
                .Count());
        }

        [Test]
        [Deployment]
        public virtual void testCompensationCreation()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensationProcess");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("innerTask")
                .Execute();

            var task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "innerTask")
                .First();
            var task = taskService.CreateTaskQuery(c=>c.ExecutionId ==task2Execution.Id)
                .First();
            Assert.NotNull(task);

            taskService.Complete(task.Id);
            Assert.AreEqual(3, runtimeService.CreateEventSubscriptionQuery()
                .Count());

            // trigger compensation
            var outerTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "outerTask")
                .First();
            Assert.NotNull(outerTask);
            taskService.Complete(outerTask.Id);

            // then there are two compensation tasks and the afterSubprocessTask:
            Assert.AreEqual(3, taskService.CreateTaskQuery()
                .Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "innerAfterBoundaryTask")
                .Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "outerAfterBoundaryTask")
                .Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "taskAfterSubprocess")
                .Count());

            // complete process
            completeTasksInOrder("taskAfterSubprocess", "innerAfterBoundaryTask", "outerAfterBoundaryTask");

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testNoCompensationCreatedOnCancellation()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensationProcess");
            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            // one on outerTask, one on innerTask
            Assert.AreEqual(2, taskService.CreateTaskQuery()
                .Count());

            // when inner task is cancelled
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "innerTask"))
                .Execute();

            // then no compensation event subscription exists
            Assert.AreEqual(0, runtimeService.CreateEventSubscriptionQuery()
                .Count());

            // and the compensation throw event does not trigger compensation handlers
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);
            Assert.AreEqual("outerTask", task.TaskDefinitionKey);

            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testStartActivityInTransactionWithCompensation()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            completeTasksInOrder("userTask");

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("undoTask", task.TaskDefinitionKey);

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("tx").Activity("txEnd").Activity("undoTask").Done());

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("userTask")
                .Execute();

            tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("tx").Activity("txEnd").Activity("undoTask").Activity("userTask").Done());

            completeTasksInOrder("userTask");

            tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("tx").Activity("txEnd").Activity("undoTask").Done());

            var NewTask = taskService.CreateTaskQuery()
                .First();
            Assert.AreNotSame(task.Id, NewTask.Id);

            completeTasksInOrder("undoTask", "afterCancel");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testStartActivityWithAncestorInTransactionWithCompensation()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            completeTasksInOrder("userTask");

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("undoTask", task.TaskDefinitionKey);

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("tx").Activity("txEnd").Activity("undoTask").Done());

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("userTask", processInstance.Id)
                .Execute();

            completeTasksInOrder("userTask");

            tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("tx").Activity("txEnd").Activity("undoTask").EndScope().BeginScope("tx").Activity("txEnd").Activity("undoTask").Done());

            completeTasksInOrder("undoTask", "undoTask", "afterCancel", "afterCancel");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testStartAfterActivityDuringCompensation()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            completeTasksInOrder("userTask");

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("undoTask", task.TaskDefinitionKey);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartAfterActivity("userTask")
                .Execute();

            task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("afterCancel", task.TaskDefinitionKey);

            completeTasksInOrder("afterCancel");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testCancelCompensatingTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            completeTasksInOrder("userTask");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "undoTask"))
                .Execute();

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testCancelCompensatingTaskAndStartActivity()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            completeTasksInOrder("userTask");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "undoTask"))
                .StartBeforeActivity("userTask")
                .Execute();

            tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("tx").Activity("userTask").Done());

            completeTasksInOrder("userTask", "undoTask", "afterCancel");

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testCancelCompensatingTaskAndStartActivityWithAncestor()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            completeTasksInOrder("userTask");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(getInstanceIdForActivity(tree, "undoTask"))
                .StartBeforeActivity("userTask", processInstance.Id)
                .Execute();

            tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("tx").Activity("userTask").Done());

            completeTasksInOrder("userTask", "undoTask", "afterCancel");

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testStartActivityAndCancelCompensatingTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            completeTasksInOrder("userTask");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("userTask")
                .CancelActivityInstance(getInstanceIdForActivity(tree, "undoTask"))
                .Execute();

            tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("tx").Activity("userTask").Done());

            completeTasksInOrder("userTask", "undoTask", "afterCancel");

            AssertProcessEnded(processInstance.Id);
        }
        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testStartCompensatingTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("undoTask")
                .Execute();

            completeTasksInOrder("undoTask");

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("userTask", task.TaskDefinitionKey);

            completeTasksInOrder("userTask", "undoTask", "afterCancel");

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testStartAdditionalCompensatingTaskAndCompleteOldCompensationTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            completeTasksInOrder("userTask");

            var firstUndoTask = taskService.CreateTaskQuery()
                .First();

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("undoTask")
                .Execute();

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("tx").Activity("txEnd").Activity("undoTask").Activity("undoTask").Done());

            taskService.Complete(firstUndoTask.Id);

            var secondUndoTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "undoTask")
                .First();
            Assert.IsNull(secondUndoTask);

            completeTasksInOrder("afterCancel");

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testStartAdditionalCompensatingTaskAndCompleteNewCompensatingTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            completeTasksInOrder("userTask");

            var firstUndoTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "undoTask")
                .First();

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("undoTask")
                .SetVariableLocal("new", true)
                .Execute();

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("tx").Activity("txEnd").Activity("undoTask").Activity("undoTask").Done());

            var taskExecutionId = runtimeService.CreateExecutionQuery()
                ////.VariableValueEquals("new", true)
                .First()
                .Id;
            var secondUndoTask = taskService.CreateTaskQuery(c=>c.ExecutionId ==taskExecutionId)
                .First();

            Assert.NotNull(secondUndoTask);
            Assert.AreNotSame(firstUndoTask.Id, secondUndoTask.Id);
            taskService.Complete(secondUndoTask.Id);

            tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("tx").Activity("txEnd").Activity("undoTask").Done());

            completeTasksInOrder("undoTask", "afterCancel");

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testStartCompensationBoundary()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartBeforeActivity("compensateBoundaryEvent")
                    .Execute();

                Assert.Fail("should not succeed");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("compensation boundary event", e.Message);
            }

            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartAfterActivity("compensateBoundaryEvent")
                    .Execute();

                Assert.Fail("should not succeed");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("no outgoing sequence flow", e.Message);
            }
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testStartCancelEndEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            completeTasksInOrder("userTask");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("txEnd")
                .Execute();

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("afterCancel", task.TaskDefinitionKey);

            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testStartCancelBoundaryEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            completeTasksInOrder("userTask");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("catchCancelTx")
                .Execute();

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("afterCancel", task.TaskDefinitionKey);

            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(TRANSACTION_WITH_COMPENSATION_PROCESS)]
        public virtual void testStartTaskAfterCancelBoundaryEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");
            completeTasksInOrder("userTask");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("afterCancel")
                .Execute();

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("tx").Activity("txEnd").Activity("undoTask").EndScope().Activity("afterCancel").Done());

            completeTasksInOrder("afterCancel", "undoTask", "afterCancel");

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testCancelNonExistingActivityInstance()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");

            // when - then throw exception
            try
            {
                runtimeService.CreateProcessInstanceModification(instance.Id)
                    .CancelActivityInstance("nonExistingActivityInstance")
                    .Execute();
                Assert.Fail("should not succeed");
            }
            catch (NotValidException e)
            {
                AssertTextPresent(
                    "Cannot perform instruction: Cancel activity instance 'nonExistingActivityInstance'; " +
                    "Activity instance 'nonExistingActivityInstance' does not exist", e.Message);
            }
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testCancelNonExistingTranisitionInstance()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");

            // when - then throw exception
            try
            {
                runtimeService.CreateProcessInstanceModification(instance.Id)
                    .CancelTransitionInstance("nonExistingActivityInstance")
                    .Execute();
                Assert.Fail("should not succeed");
            }
            catch (NotValidException e)
            {
                AssertTextPresent(
                    "Cannot perform instruction: Cancel transition instance 'nonExistingActivityInstance'; " +
                    "Transition instance 'nonExistingActivityInstance' does not exist", e.Message);
            }
        }

        [Test]
        [Deployment(new string[] { CALL_ACTIVITY_PARENT_PROCESS, CALL_ACTIVITY_CHILD_PROCESS })]
        public virtual void FAILING_testCancelCallActivityInstance()
        {
            // given
            var parentprocess = runtimeService.StartProcessInstanceByKey("parentprocess");
            var subProcess = runtimeService.CreateProcessInstanceQuery()
                ////.SetProcessDefinitionKey("subprocess")
                .First();

            var subProcessActivityInst = runtimeService.GetActivityInstance(subProcess.Id);

            // when
            runtimeService.CreateProcessInstanceModification(subProcess.Id)
                .StartBeforeActivity("childEnd", subProcess.Id)
                .CancelActivityInstance(getInstanceIdForActivity(subProcessActivityInst, "innerTask"))
                .Execute();

            // then
            AssertProcessEnded(parentprocess.Id);
        }

        [Test]
        public virtual void testModifyNullProcessInstance()
        {
            try
            {
                runtimeService.CreateProcessInstanceModification(null)
                    .StartBeforeActivity("someActivity")
                    .Execute();
                Assert.Fail("should not succeed");
            }
            catch (NotValidException e)
            {
                AssertTextPresent("ProcessInstanceId is null", e.Message);
            }
        }

        // TODO: check if starting with a non-existing activity/transition id is
        // handled properly

        protected internal virtual string getInstanceIdForActivity(IActivityInstance activityInstance, string activityId)
        {
            var instance = getChildInstanceForActivity(activityInstance, activityId);
            if (instance != null)
                return instance.Id;
            return null;
        }

        protected internal virtual IActivityInstance getChildInstanceForActivity(IActivityInstance activityInstance,
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