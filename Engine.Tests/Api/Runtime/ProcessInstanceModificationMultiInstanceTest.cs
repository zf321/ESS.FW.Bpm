using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ProcessInstanceModificationMultiInstanceTest : PluggableProcessEngineTestCase
    {
        public const string PARALLEL_MULTI_INSTANCE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationMultiInstanceTest.parallelTasks.bpmn20.xml";

        public const string PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationMultiInstanceTest.parallelSubprocess.bpmn20.xml";

        public const string SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationMultiInstanceTest.sequentialTasks.bpmn20.xml";

        public const string SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationMultiInstanceTest.sequentialSubprocess.bpmn20.xml";

        public const string PARALLEL_MULTI_INSTANCE_TASK_COMPLETION_CONDITION_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationMultiInstanceTest.parallelTasksCompletionCondition.bpmn20.xml";

        public const string PARALLEL_MULTI_INSTANCE_SUBPROCESS_COMPLETION_CONDITION_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationMultiInstanceTest.parallelSubprocessCompletionCondition.bpmn20.xml";

        public const string NESTED_PARALLEL_MULTI_INSTANCE_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationMultiInstanceTest.NestedParallelTasks.bpmn20.xml";

        [Test]
        [Deployment( PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testStartBeforeMultiInstanceBodyParallelTasks()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelUserTasks");
            completeTasksInOrder("beforeTask");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("miTasks#multiInstanceBody")
                .Execute();

            // then
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginMiBody("miTasks")
                    .Activity("miTasks")
                    .Activity("miTasks")
                    .Activity("miTasks")
                    .EndScope()
                    .BeginMiBody("miTasks")
                    .Activity("miTasks")
                    .Activity("miTasks")
                    .Activity("miTasks")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("miTasks", "miTasks", "miTasks", "miTasks", "miTasks", "miTasks", "afterTask",
                "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS) ]
        public virtual void testStartBeforeMultiInstanceBodyParallelSubprocess()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelSubprocess");
            completeTasksInOrder("beforeTask");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("miSubProcess#multiInstanceBody")
                .Execute();

            // then
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginMiBody("miSubProcess")
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .EndScope()
                    .BeginMiBody("miSubProcess")
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("subProcessTask")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("subProcessTask")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("subProcessTask")
                    .Scope()
                    .Up()
                    .Up()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("subProcessTask")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("subProcessTask")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("subProcessTask")
                    .Scope()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("subProcessTask", "subProcessTask", "subProcessTask", "subProcessTask",
                "subProcessTask", "afterTask", "subProcessTask", "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment( PARALLEL_MULTI_INSTANCE_TASK_COMPLETION_CONDITION_PROCESS) ]
        public virtual void testStartInnerActivityParallelTasksWithCompletionCondition()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelUserTasksCompletionCondition");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("miTasks")
                .StartBeforeActivity("miTasks")
                .Execute();

            // then the process is able to complete successfully and respects the completion condition
            completeTasksInOrder("miTasks", "miTasks", "miTasks", "miTasks");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_SUBPROCESS_COMPLETION_CONDITION_PROCESS)]
        public virtual void testStartInnerActivityParallelSubprocessWithCompletionCondition()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelSubprocessCompletionCondition");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("miSubProcess")
                .StartBeforeActivity("miSubProcess")
                .Execute();

            // then the process is able to complete successfully and respects the completion condition
            completeTasksInOrder("subProcessTask", "subProcessTask", "subProcessTask", "subProcessTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS) ]
        public virtual void testStartBeforeMultiInstanceBodySequentialTasks()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialUserTasks");
            completeTasksInOrder("beforeTask");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("miTasks#multiInstanceBody")
                .Execute();

            // then
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginMiBody("miTasks")
                    .Activity("miTasks")
                    .EndScope()
                    .BeginMiBody("miTasks")
                    .Activity("miTasks")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("miTasks")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("miTasks")
                    .Scope()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("miTasks", "miTasks", "miTasks", "miTasks", "miTasks", "miTasks", "afterTask",
                "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS) ]
        public virtual void testStartBeforeMultiInstanceBodySequentialSubprocess()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialSubprocess");
            completeTasksInOrder("beforeTask");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("miSubProcess#multiInstanceBody")
                .Execute();

            // then
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginMiBody("miSubProcess")
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .EndScope()
                    .BeginMiBody("miSubProcess")
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("subProcessTask")
                    .Scope()
                    .Up()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("subProcessTask")
                    .Scope()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("subProcessTask", "subProcessTask", "subProcessTask", "subProcessTask",
                "subProcessTask", "subProcessTask", "afterTask", "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testStartBeforeInnerActivityParallelTasks()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelUserTasks");
            completeTasksInOrder("beforeTask");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("miTasks", tree.GetActivityInstances("miTasks#multiInstanceBody")[0].Id)
                .Execute();

            // then the mi variables should be correct
            var leafExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "miTasks")
                
                .ToList();
            Assert.AreEqual(4, leafExecutions.Count);
            AssertVariableSet(leafExecutions, "loopCounter", new List<int> {0, 1, 2, 3});
            foreach (var leafExecution in leafExecutions)
            {
                AssertVariable(leafExecution, "nrOfInstances", 4);
                AssertVariable(leafExecution, "nrOfCompletedInstances", 0);
                AssertVariable(leafExecution, "nrOfActiveInstances", 4);
            }

            // and the trees should be correct
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginMiBody("miTasks")
                    .Activity("miTasks")
                    .Activity("miTasks")
                    .Activity("miTasks")
                    .Activity("miTasks")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("miTasks", "miTasks", "miTasks", "miTasks", "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
        public virtual void testStartBeforeInnerActivityParallelSubprocess()
        {
            // given the mi body is already instantiated
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelSubprocess");
            completeTasksInOrder("beforeTask");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("miSubProcess", tree.GetActivityInstances("miSubProcess#multiInstanceBody")[0].Id)
                .Execute();

            // then the mi variables should be correct
            var leafExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "subProcessTask")
                
                .ToList();
            Assert.AreEqual(4, leafExecutions.Count);
            AssertVariableSet(leafExecutions, "loopCounter", new List<int> {0, 1, 2, 3});
            foreach (var leafExecution in leafExecutions)
            {
                AssertVariable(leafExecution, "nrOfInstances", 4);
                AssertVariable(leafExecution, "nrOfCompletedInstances", 0);
                AssertVariable(leafExecution, "nrOfActiveInstances", 4);
            }

            // and the trees are correct
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginMiBody("miSubProcess")
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("subProcessTask")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("subProcessTask")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("subProcessTask")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("subProcessTask")
                    .Scope()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("subProcessTask", "subProcessTask", "subProcessTask", "subProcessTask", "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testStartBeforeInnerActivityWithMiBodyParallelTasks()
        {
            // given the mi body is not yet instantiated
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelUserTasks");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("miTasks")
                .Execute();

            // then the mi variables should be correct
            var leafExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "miTasks")
                .First();
            Assert.NotNull(leafExecution);
            AssertVariable(leafExecution, "loopCounter", 0);
            AssertVariable(leafExecution, "nrOfInstances", 1);
            AssertVariable(leafExecution, "nrOfCompletedInstances", 0);
            AssertVariable(leafExecution, "nrOfActiveInstances", 1);

            // and the tree should be correct
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("beforeTask")
                    .BeginMiBody("miTasks")
                    .Activity("miTasks")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("beforeTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("miTasks", "afterTask", "beforeTask", "miTasks", "miTasks", "miTasks", "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testStartBeforeInnerActivityWithMiBodyParallelTasksActivityStatistics()
        {
            // given the mi body is not yet instantiated
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelUserTasks");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("miTasks")
                .Execute();

            // then the activity instance statistics are correct
            var statistics = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId)
                
                .ToList();
            Assert.AreEqual(2, statistics.Count);

            var miTasksStatistics = getStatisticsForActivity(statistics, "miTasks");
            Assert.NotNull(miTasksStatistics);
            Assert.AreEqual(1, miTasksStatistics.Instances);

            var beforeTaskStatistics = getStatisticsForActivity(statistics, "beforeTask");
            Assert.NotNull(beforeTaskStatistics);
            Assert.AreEqual(1, beforeTaskStatistics.Instances);
        }

        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
        public virtual void testStartBeforeInnerActivityWithMiBodyParallelSubprocess()
        {
            // given the mi body is not yet instantiated
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelSubprocess");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("subProcessTask")
                .Execute();

            // then the mi variables should be correct
            var leafExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "subProcessTask")
                .First();
            Assert.NotNull(leafExecution);
            AssertVariable(leafExecution, "loopCounter", 0);
            AssertVariable(leafExecution, "nrOfInstances", 1);
            AssertVariable(leafExecution, "nrOfCompletedInstances", 0);
            AssertVariable(leafExecution, "nrOfActiveInstances", 1);

            // and the tree should be correct
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("beforeTask")
                    .BeginMiBody("miSubProcess")
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("beforeTask")
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
                    .Child("subProcessTask")
                    .Scope()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("subProcessTask", "afterTask", "beforeTask", "subProcessTask", "subProcessTask",
                "subProcessTask", "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
        public virtual void testStartBeforeInnerActivityWithMiBodyParallelSubprocessActivityStatistics()
        {
            // given the mi body is not yet instantiated
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelSubprocess");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("subProcessTask")
                .Execute();

            // then the activity instance statistics are correct
            var statistics = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId)
                
                .ToList();
            Assert.AreEqual(2, statistics.Count);

            var miTasksStatistics = getStatisticsForActivity(statistics, "subProcessTask");
            Assert.NotNull(miTasksStatistics);
            Assert.AreEqual(1, miTasksStatistics.Instances);

            var beforeTaskStatistics = getStatisticsForActivity(statistics, "beforeTask");
            Assert.NotNull(beforeTaskStatistics);
            Assert.AreEqual(1, beforeTaskStatistics.Instances);
        }


        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
        public virtual void testStartBeforeInnerActivityWithMiBodySetNrOfInstancesParallelSubprocess()
        {
            // given the mi body is not yet instantiated
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelSubprocess");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("subProcessTask")
                .SetVariable("nrOfInstances", 3)
                .Execute();

            // then the mi variables should be correct
            var leafExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "subProcessTask")
                .First();
            Assert.NotNull(leafExecution);
            AssertVariable(leafExecution, "loopCounter", 0);
            AssertVariable(leafExecution, "nrOfInstances", 3);
            AssertVariable(leafExecution, "nrOfCompletedInstances", 0);
            AssertVariable(leafExecution, "nrOfActiveInstances", 1);

            // and the trees should be correct
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("beforeTask")
                    .BeginMiBody("miSubProcess")
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("beforeTask")
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
                    .Child("subProcessTask")
                    .Scope()
                    .Done());

            // and completing the single active instance completes the mi body (even though nrOfInstances is 3;
            // joining is performed on the number of concurrent executions)
            completeTasksInOrder("subProcessTask");
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("beforeTask")
                    .Activity("afterTask")
                    .Done());

            // and the remainder of the process completes successfully
            completeTasksInOrder("beforeTask", "subProcessTask", "afterTask", "subProcessTask", "subProcessTask",
                "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS) ]
        public virtual void testStartBeforeInnerActivitySequentialTasks()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialUserTasks");
            completeTasksInOrder("beforeTask");

            // then creating a second inner instance is not possible
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartBeforeActivity("miTasks", tree.GetActivityInstances("miTasks#multiInstanceBody")[0].Id)
                    .Execute();
                Assert.Fail("expect exception");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent(e.Message,
                    "Concurrent instantiation not possible for activities " + "in scope miTasks#multiInstanceBody");
            }
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
        public virtual void testStartBeforeInnerActivitySequentialSubprocess()
        {
            // given the mi body is already instantiated
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialSubprocess");
            completeTasksInOrder("beforeTask");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            try
            {
                runtimeService.CreateProcessInstanceModification(processInstance.Id)
                    .StartBeforeActivity("miSubProcess",
                        tree.GetActivityInstances("miSubProcess#multiInstanceBody")[0].Id)
                    .Execute();
                Assert.Fail("expect exception");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent(e.Message,
                    "Concurrent instantiation not possible for activities " + "in scope miSubProcess#multiInstanceBody");
            }
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testStartBeforeInnerActivityWithMiBodySequentialTasks()
        {
            // given the mi body is not yet instantiated
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialUserTasks");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("miTasks")
                .Execute();

            // then the mi variables should be correct
            var leafExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "miTasks")
                .First();
            Assert.NotNull(leafExecution);
            AssertVariable(leafExecution, "loopCounter", 0);
            AssertVariable(leafExecution, "nrOfInstances", 1);
            AssertVariable(leafExecution, "nrOfCompletedInstances", 0);
            AssertVariable(leafExecution, "nrOfActiveInstances", 1);

            // and the trees should be correct
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("beforeTask")
                    .BeginMiBody("miTasks")
                    .Activity("miTasks")
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("beforeTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("miTasks")
                    .Scope()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("miTasks", "afterTask", "beforeTask", "miTasks", "miTasks", "miTasks", "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testStartBeforeInnerActivityWithMiBodySequentialTasksActivityStatistics()
        {
            // given the mi body is not yet instantiated
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialUserTasks");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("miTasks")
                .Execute();

            // then the activity instance statistics are correct
            var statistics = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId)
                
                .ToList();
            Assert.AreEqual(2, statistics.Count);

            var miTasksStatistics = getStatisticsForActivity(statistics, "miTasks");
            Assert.NotNull(miTasksStatistics);
            Assert.AreEqual(1, miTasksStatistics.Instances);

            var beforeTaskStatistics = getStatisticsForActivity(statistics, "beforeTask");
            Assert.NotNull(beforeTaskStatistics);
            Assert.AreEqual(1, beforeTaskStatistics.Instances);
        }

        protected internal virtual IActivityStatistics getStatisticsForActivity(IList<IActivityStatistics> statistics,
            string activityId)
        {
            foreach (var statisticsInstance in statistics)
                if (statisticsInstance.Id.Equals(activityId))
                    return statisticsInstance;
            return null;
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
        public virtual void testStartBeforeInnerActivityWithMiBodySequentialSubprocess()
        {
            // given the mi body is not yet instantiated
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialSubprocess");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("subProcessTask")
                .Execute();

            // then the mi variables should be correct
            var leafExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "subProcessTask")
                .First();
            Assert.NotNull(leafExecution);
            AssertVariable(leafExecution, "loopCounter", 0);
            AssertVariable(leafExecution, "nrOfInstances", 1);
            AssertVariable(leafExecution, "nrOfCompletedInstances", 0);
            AssertVariable(leafExecution, "nrOfActiveInstances", 1);

            // and the trees should be correct
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("beforeTask")
                    .BeginMiBody("miSubProcess")
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("beforeTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("subProcessTask")
                    .Scope()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("subProcessTask", "afterTask", "beforeTask", "subProcessTask", "subProcessTask",
                "subProcessTask", "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
        public virtual void testStartBeforeInnerActivityWithMiBodySequentialSubprocessActivityStatistics()
        {
            // given the mi body is not yet instantiated
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialSubprocess");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("subProcessTask")
                .Execute();

            // then the activity instance statistics are correct
            var statistics = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId)
                
                .ToList();
            Assert.AreEqual(2, statistics.Count);

            var miTasksStatistics = getStatisticsForActivity(statistics, "subProcessTask");
            Assert.NotNull(miTasksStatistics);
            Assert.AreEqual(1, miTasksStatistics.Instances);

            var beforeTaskStatistics = getStatisticsForActivity(statistics, "beforeTask");
            Assert.NotNull(beforeTaskStatistics);
            Assert.AreEqual(1, beforeTaskStatistics.Instances);
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
        public virtual void testStartBeforeInnerActivityWithMiBodySetNrOfInstancesSequentialSubprocess()
        {
            // given the mi body is not yet instantiated
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialSubprocess");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("subProcessTask")
                .SetVariable("nrOfInstances", 3)
                .Execute();

            // then the mi variables should be correct
            var leafExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "subProcessTask")
                .First();
            Assert.NotNull(leafExecution);
            AssertVariable(leafExecution, "loopCounter", 0);
            AssertVariable(leafExecution, "nrOfInstances", 3);
            AssertVariable(leafExecution, "nrOfCompletedInstances", 0);
            AssertVariable(leafExecution, "nrOfActiveInstances", 1);

            // and the trees should be correct
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("beforeTask")
                    .BeginMiBody("miSubProcess")
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child("beforeTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("subProcessTask")
                    .Scope()
                    .Done());

            // and two following sequential instances should be created
            completeTasksInOrder("subProcessTask");

            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("beforeTask")
                    .BeginMiBody("miSubProcess")
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .Done());

            leafExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "subProcessTask")
                .First();
            Assert.NotNull(leafExecution);
            AssertVariable(leafExecution, "loopCounter", 1);
            AssertVariable(leafExecution, "nrOfInstances", 3);
            AssertVariable(leafExecution, "nrOfCompletedInstances", 1);
            AssertVariable(leafExecution, "nrOfActiveInstances", 1);

            completeTasksInOrder("subProcessTask");

            // and the remainder of the process completes successfully
            completeTasksInOrder("subProcessTask", "beforeTask", "subProcessTask", "subProcessTask", "subProcessTask",
                "afterTask", "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testCancelMultiInstanceBodyParallelTasks()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelUserTasks");
            completeTasksInOrder("beforeTask");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelAllForActivity("miTasks#multiInstanceBody")
                .Execute();

            // then
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
        public virtual void testCancelMultiInstanceBodyParallelSubprocess()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelSubprocess");
            completeTasksInOrder("beforeTask");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelAllForActivity("miSubProcess#multiInstanceBody")
                .Execute();

            // then
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testCancelMultiInstanceBodySequentialTasks()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialUserTasks");
            completeTasksInOrder("beforeTask");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelAllForActivity("miTasks#multiInstanceBody")
                .Execute();

            // then
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
        public virtual void testCancelMultiInstanceBodySequentialSubprocess()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialSubprocess");
            completeTasksInOrder("beforeTask");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelAllForActivity("miSubProcess#multiInstanceBody")
                .Execute();

            // then
            AssertProcessEnded(processInstance.Id);
        }
        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testCancelInnerActivityParallelTasks()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelUserTasks");
            completeTasksInOrder("beforeTask");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(tree.GetActivityInstances("miTasks")[0].Id)
                .Execute();

            // then
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginMiBody("miTasks")
                    .Activity("miTasks")
                    .Activity("miTasks")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("miTasks", "miTasks", "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testCancelAllInnerActivityParallelTasks()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelUserTasks");
            completeTasksInOrder("beforeTask");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelAllForActivity("miTasks")
                .Execute();

            // then
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(NESTED_PARALLEL_MULTI_INSTANCE_TASK_PROCESS) ]
        public virtual void testCancelAllInnerActivityNestedParallelTasks()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedMiParallelUserTasks");
            completeTasksInOrder("beforeTask");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelAllForActivity("miTasks")
                .Execute();

            // then
            AssertProcessEnded(processInstance.Id);
        }

        /// <summary>
        ///     Ensures that the modification cmd does not prune the last concurrent execution
        ///     because parallel MI requires this
        /// </summary>
        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testCancelInnerActivityParallelTasksAllButOne()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelUserTasks");
            completeTasksInOrder("beforeTask");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(tree.GetActivityInstances("miTasks")[0].Id)
                .CancelActivityInstance(tree.GetActivityInstances("miTasks")[1].Id)
                .Execute();

            // then
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginMiBody("miTasks")
                    .Activity("miTasks")
                    .EndScope()
                    .Done());

            // the execution tree should still be in the expected shape
            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child("miTasks")
                    .Concurrent()
                    .NoScope()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("miTasks", "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(PARALLEL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
        public virtual void testCancelInnerActivityParallelSubprocess()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miParallelSubprocess");
            completeTasksInOrder("beforeTask");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(tree.GetActivityInstances("miSubProcess")[0].Id)
                .Execute();

            // then
            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginMiBody("miSubProcess")
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .BeginScope("miSubProcess")
                    .Activity("subProcessTask")
                    .EndScope()
                    .Done());

            var executionTree = ExecutionTree.ForExecution(processInstance.Id, ProcessEngine);
            ExecutionAssert.That(executionTree)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Child(null)
                    .Scope()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("subProcessTask")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("subProcessTask")
                    .Scope()
                    .Up()
                    .Up()
                    .Done());

            // and the process is able to complete successfully
            completeTasksInOrder("subProcessTask", "subProcessTask", "afterTask");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testCancelInnerActivitySequentialTasks()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialUserTasks");
            completeTasksInOrder("beforeTask");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(tree.GetActivityInstances("miTasks")[0].Id)
                .Execute();

            // then
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_TASK_PROCESS)]
        public virtual void testCancelAllInnerActivitySequentialTasks()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialUserTasks");
            completeTasksInOrder("beforeTask");

            // when
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelAllForActivity("miTasks")
                .Execute();

            // then
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(SEQUENTIAL_MULTI_INSTANCE_SUBPROCESS_PROCESS)]
        public virtual void testCancelInnerActivitySequentialSubprocess()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialSubprocess");
            completeTasksInOrder("beforeTask");

            // when
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .CancelActivityInstance(tree.GetActivityInstances("miSubProcess")[0].Id)
                .Execute();

            // then
            AssertProcessEnded(processInstance.Id);
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


        protected internal virtual void AssertVariable(IExecution execution, string VariableName, object expectedValue)
        {
            var variableValue = runtimeService.GetVariable(execution.Id, VariableName);
            Assert.AreEqual(variableValue, expectedValue,
                "Value for variable '" + VariableName + "' and " + execution + " " + "does not match."
            );
        }

        protected internal virtual void AssertVariableSet<T1>(IList<IExecution> executions, string VariableName,
            IList<T1> expectedValues)
        {
            IList<object> actualValues = new List<object>();
            foreach (var execution in executions)
                actualValues.Add(runtimeService.GetVariable(execution.Id, VariableName));

            foreach (object expectedValue in expectedValues)
            {
                var valueFound = actualValues.Remove(expectedValue);
                Assert.True(valueFound,
                    "Expected variable value '" + expectedValue + "' not contained in the list of actual values. " +
                    "Unmatched actual values: " + actualValues);
            }
            Assert.True(actualValues.Count == 0, "There are more actual than expected values.");
        }
    }
}