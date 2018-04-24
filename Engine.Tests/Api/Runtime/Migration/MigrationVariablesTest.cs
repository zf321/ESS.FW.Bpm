using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationVariablesTest
    {
        [SetUp]
        public virtual void initServices()
        {
            runtimeService = rule.RuntimeService;
            taskService = rule.TaskService;
        }

        protected internal static readonly IBpmnModelInstance ONE_BOUNDARY_TASK =
                ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
            //.ActivityBuilder("userTask")
            //.BoundaryEvent()
            //.Message("Message")
            //.Done()
            ;

        protected internal static readonly IBpmnModelInstance CONCURRENT_BOUNDARY_TASKS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
            //.ActivityBuilder("userTask1")
            //.BoundaryEvent()
            //.Message("Message")
            //.MoveToActivity("userTask2")
            //.BoundaryEvent()
            //.Message("Message")
            //.Done()
            ;

        protected internal static readonly IBpmnModelInstance SUBPROCESS_CONCURRENT_BOUNDARY_TASKS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewaySubprocessProcess)
            //.ActivityBuilder("userTask1")
            //.BoundaryEvent()
            //.Message("Message")
            //.MoveToActivity("userTask2")
            //.BoundaryEvent()
            //.Message("Message")
            //.Done()
            ;

        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal ITaskService taskService;
        protected internal MigrationTestRule testHelper;

        public MigrationVariablesTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testHelper = new MigrationTestRule(rule);
            //ruleChain = RuleChain.outerRule(rule).around(testHelper);
        }

        [Test]
        public virtual void testVariableAtScopeExecutionInScopeActivity()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ONE_BOUNDARY_TASK);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ONE_BOUNDARY_TASK);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var scopeExecution = executionTreeBeforeMigration.Executions[0];

            runtimeService.SetVariableLocal(scopeExecution.Id, "foo", 42);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);
        }

        [Test]
        public virtual void testVariableAtConcurrentExecutionInScopeActivity()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(CONCURRENT_BOUNDARY_TASKS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(CONCURRENT_BOUNDARY_TASKS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var concurrentExecution = executionTreeBeforeMigration.Executions[0];

            runtimeService.SetVariableLocal(concurrentExecution.Id, "foo", 42);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);
        }

        [Test]
        public virtual void testVariableAtScopeExecutionInNonScopeActivity()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id
                //Variable.Variables.CreateVariables()
                //    .PutValue("foo",new IntegerValueImpl( 42))
            );

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);
        }

        [Test]
        public virtual void testVariableAtConcurrentExecutionInNonScopeActivity()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var concurrentExecution = executionTreeBeforeMigration.Executions[0];

            runtimeService.SetVariableLocal(concurrentExecution.Id, "foo", 42);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);
        }

        [Test]
        public virtual void testVariableAtConcurrentExecutionInScopeActivityAddParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(CONCURRENT_BOUNDARY_TASKS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(SUBPROCESS_CONCURRENT_BOUNDARY_TASKS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var userTask1CCExecutionBefore = executionTreeBeforeMigration.GetLeafExecutions("userTask1")[0].Parent;

            runtimeService.SetVariableLocal(userTask1CCExecutionBefore.Id, "foo", 42);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");

            var userTask1CCExecutionAfter =
                testHelper.SnapshotAfterMigration.ExecutionTree.GetLeafExecutions("userTask1")[0].Parent;

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            var subProcessInstance = testHelper.GetSingleActivityInstanceAfterMigration("subProcess");
            // for variables at concurrent executions that are parent of a leaf-scope-execution, the activity instance is
            // the activity instance id of the parent activity instance (which is probably a bug)
            testHelper.AssertVariableMigratedToExecution(beforeMigration, userTask1CCExecutionAfter.Id,
                subProcessInstance.Id);
        }

        [Test]
        public virtual void testVariableAtConcurrentExecutionInScopeActivityRemoveParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(SUBPROCESS_CONCURRENT_BOUNDARY_TASKS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(CONCURRENT_BOUNDARY_TASKS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var userTask1CCExecutionBefore = executionTreeBeforeMigration.GetLeafExecutions("userTask1")[0].Parent;

            runtimeService.SetVariableLocal(userTask1CCExecutionBefore.Id, "foo", 42);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");

            var userTask1CCExecutionAfter =
                testHelper.SnapshotAfterMigration.ExecutionTree.GetLeafExecutions("userTask1")[0].Parent;

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            // for variables at concurrent executions that are parent of a leaf-scope-execution, the activity instance is
            // the activity instance id of the parent activity instance (which is probably a bug)
            testHelper.AssertVariableMigratedToExecution(beforeMigration, userTask1CCExecutionAfter.Id,
                processInstance.Id);
        }

        [Test]
        public virtual void testVariableAtConcurrentExecutionInNonScopeActivityAddParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewaySubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var userTask1CCExecutionBefore = executionTreeBeforeMigration.GetLeafExecutions("userTask1")[0];

            runtimeService.SetVariableLocal(userTask1CCExecutionBefore.Id, "foo", 42);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");

            var userTask1CCExecutionAfter =
                testHelper.SnapshotAfterMigration.ExecutionTree.GetLeafExecutions("userTask1")[0];

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(beforeMigration, userTask1CCExecutionAfter.Id);
        }

        [Test]
        public virtual void testVariableAtConcurrentExecutionInNonScopeActivityRemoveParentScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewaySubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var userTask1CCExecutionBefore = executionTreeBeforeMigration.GetLeafExecutions("userTask1")[0];

            runtimeService.SetVariableLocal(userTask1CCExecutionBefore.Id, "foo", 42);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");

            var userTask1CCExecutionAfter =
                testHelper.SnapshotAfterMigration.ExecutionTree.GetLeafExecutions("userTask1")[0];

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(beforeMigration, userTask1CCExecutionAfter.Id);
        }

        [Test]
        public virtual void testVariableAtScopeExecutionInScopeActivityAddParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ONE_BOUNDARY_TASK);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(SUBPROCESS_CONCURRENT_BOUNDARY_TASKS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask1")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var scopeExecution = executionTreeBeforeMigration.Executions[0];

            runtimeService.SetVariableLocal(scopeExecution.Id, "foo", 42);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);
        }

        [Test]
        public virtual void testVariableAtTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.SetVariableLocal(task.Id, "foo", 42);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);
        }

        [Test]
        public virtual void testVariableAtTaskAddParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewaySubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "userTask1")
                .First();
            taskService.SetVariableLocal(task.Id, "foo", 42);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");

            var userTask1ExecutionAfter =
                testHelper.SnapshotAfterMigration.ExecutionTree.GetLeafExecutions("userTask1")[0];

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(beforeMigration, userTask1ExecutionAfter.Id);
        }
        [Test]
        public virtual void testVariableAtTaskAndConcurrentExecutionAddParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewaySubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "userTask1")
                .First();
            taskService.SetVariableLocal(task.Id, "foo", 42);
            runtimeService.SetVariableLocal(task.ExecutionId, "foo", 52);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var taskVarBeforeMigration = testHelper.SnapshotBeforeMigration.GetSingleTaskVariable(task.Id, "foo");

            var userTask1ExecutionAfter =
                testHelper.SnapshotAfterMigration.ExecutionTree.GetLeafExecutions("userTask1")[0];

            Assert.AreEqual(2, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(taskVarBeforeMigration, userTask1ExecutionAfter.Id);
        }

        [Test]
        public virtual void testVariableAtScopeExecutionBecomeNonScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ONE_BOUNDARY_TASK);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var scopeExecution = executionTreeBeforeMigration.Executions[0];

            runtimeService.SetVariableLocal(scopeExecution.Id, "foo", 42);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(beforeMigration, processInstance.Id);

            // and the variable is concurrent local, i.E. expands on tree expansion
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("userTask")
                .Execute();

            var variableAfterExpansion = runtimeService.CreateVariableInstanceQuery()
                .First();
            Assert.NotNull(variableAfterExpansion);
            Assert.AreNotSame(processInstance.Id, variableAfterExpansion.ExecutionId);
        }

        [Test]
        public virtual void testVariableAtConcurrentExecutionBecomeScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var concurrentExecution = executionTreeBeforeMigration.GetLeafExecutions("userTask1")[0];

            runtimeService.SetVariableLocal(concurrentExecution.Id, "foo", 42);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");
            var userTask1CCExecution =
                testHelper.SnapshotAfterMigration.ExecutionTree.GetLeafExecutions("userTask1")[0].Parent;

            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(beforeMigration, userTask1CCExecution.Id);
        }

        [Test]
        public virtual void testVariableAtConcurrentAndScopeExecutionBecomeNonScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(CONCURRENT_BOUNDARY_TASKS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var scopeExecution = executionTreeBeforeMigration.GetLeafExecutions("userTask1")[0];
            var concurrentExecution = scopeExecution.Parent;

            runtimeService.SetVariableLocal(scopeExecution.Id, "foo", 42);
            runtimeService.SetVariableLocal(concurrentExecution.Id, "foo", 42);

            // when
            try
            {
                testHelper.MigrateProcessInstance(migrationPlan, processInstance);
                Assert.Fail("expected exception");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message,
                    Does.Contain("The variable 'foo' exists in both, this scope" +
                                        " and concurrent local in the parent scope. Migrating to a non-scope activity would overwrite one of them."));
            }
        }

        [Test]
        public virtual void testVariableAtParentScopeExecutionAndScopeExecutionBecomeNonScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ONE_BOUNDARY_TASK);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var scopeExecution = executionTreeBeforeMigration.GetLeafExecutions("userTask")[0];

            runtimeService.SetVariableLocal(scopeExecution.Id, "foo", "userTaskScopeValue");
            runtimeService.SetVariableLocal(processInstance.Id, "foo", "processScopeValue");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the process scope variable was overwritten due to a compacted execution tree
            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);

            var variable = testHelper.SnapshotAfterMigration.GetVariables()
                .GetEnumerator()
                .Current;

            Assert.AreEqual("userTaskScopeValue", variable.Value);
        }

        [Test]
        public virtual void testVariableAtConcurrentExecutionAddParentScopeBecomeNonConcurrent()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelTaskAndSubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.CamundaInputParameter("foo", "subProcessValue")
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var task1CcExecution = executionTreeBeforeMigration.GetLeafExecutions("userTask1")[0];
            var task2CcExecution = executionTreeBeforeMigration.GetLeafExecutions("userTask2")[0];

            runtimeService.SetVariableLocal(task1CcExecution.Id, "foo", "task1Value");
            runtimeService.SetVariableLocal(task2CcExecution.Id, "foo", "task2Value");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the io mapping variable was overwritten due to a compacted execution tree
            Assert.AreEqual(2, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);

            IList<string> values = new List<string>();
            foreach (var variable in testHelper.SnapshotAfterMigration.GetVariables())
                values.Add((string) variable.Value);

            Assert.True(values.Contains("task1Value"));
            Assert.True(values.Contains("task2Value"));
        }

        [Test]
        public virtual void testAddScopeWithInputMappingAndVariableOnConcurrentExecutions()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewaySubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.CamundaInputParameter("foo", "inputOutputValue")
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var userTask1CCExecutionBefore = executionTreeBeforeMigration.GetLeafExecutions("userTask1")[0];
            var userTask2CCExecutionBefore = executionTreeBeforeMigration.GetLeafExecutions("userTask2")[0];

            runtimeService.SetVariableLocal(userTask1CCExecutionBefore.Id, "foo", "customValue");
            runtimeService.SetVariableLocal(userTask2CCExecutionBefore.Id, "foo", "customValue");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the scope variable instance has been overwritten during compaction (conform to prior behavior);
            // although this is tested here, changing this behavior may be ok in the future
            var variables = testHelper.SnapshotAfterMigration.GetVariables();
            Assert.AreEqual(2, variables.Count);

            foreach (var variable in variables)
                Assert.AreEqual("customValue", variable.Value);

            var subProcessExecution =
                testHelper.SnapshotAfterMigration.ExecutionTree.GetLeafExecutions("userTask2")[0].Parent;

            Assert.NotNull(testHelper.SnapshotAfterMigration.GetSingleVariable(subProcessExecution.Id, "foo"));
        }

        [Test]
        public virtual void testVariableAtScopeAndConcurrentExecutionAddParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewaySubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var userTask1CCExecutionBefore = executionTreeBeforeMigration.GetLeafExecutions("userTask1")[0];
            var userTask2CCExecutionBefore = executionTreeBeforeMigration.GetLeafExecutions("userTask2")[0];

            runtimeService.SetVariableLocal(processInstance.Id, "foo", "processInstanceValue");
            runtimeService.SetVariableLocal(userTask1CCExecutionBefore.Id, "foo", "task1Value");
            runtimeService.SetVariableLocal(userTask2CCExecutionBefore.Id, "foo", "task2Value");

            //var processScopeVariable = runtimeService.CreateVariableInstanceQuery()
            //    //.VariableValueEquals("foo", "processInstanceValue")
            //    .First();
            //var task1Variable = runtimeService.CreateVariableInstanceQuery()
            //    //.VariableValueEquals("foo", "task1Value")
            //    .First();
            //var task2Variable = runtimeService.CreateVariableInstanceQuery()
            //    //.VariableValueEquals("foo", "task2Value")
            //    .First();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the scope variable instance has been overwritten during compaction (conform to prior behavior);
            // although this is tested here, changing this behavior may be ok in the future
            Assert.AreEqual(3, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);

            //var processScopeVariableAfterMigration =
            //    testHelper.SnapshotAfterMigration.GetVariable(processScopeVariable.Id);
            //Assert.NotNull(processScopeVariableAfterMigration);
            //Assert.AreEqual("processInstanceValue", processScopeVariableAfterMigration.Value);

            //var task1VariableAfterMigration = testHelper.SnapshotAfterMigration.GetVariable(task1Variable.Id);
            //Assert.NotNull(task1VariableAfterMigration);
            //Assert.AreEqual("task1Value", task1VariableAfterMigration.Value);

            //var task2VariableAfterMigration = testHelper.SnapshotAfterMigration.GetVariable(task2Variable.Id);
            //Assert.NotNull(task2VariableAfterMigration);
            //Assert.AreEqual("task2Value", task2VariableAfterMigration.Value);
        }

        [Test]
        public virtual void testVariableAtScopeAndConcurrentExecutionRemoveParentScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewaySubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var executionTreeBeforeMigration = ExecutionTree.ForExecution(processInstance.Id, rule.ProcessEngine);

            var userTask1CCExecutionBefore = executionTreeBeforeMigration.GetLeafExecutions("userTask1")[0];
            var userTask2CCExecutionBefore = executionTreeBeforeMigration.GetLeafExecutions("userTask2")[0];
            var subProcessExecution = userTask1CCExecutionBefore.Parent;

            runtimeService.SetVariableLocal(subProcessExecution.Id, "foo", "subProcessValue");
            runtimeService.SetVariableLocal(userTask1CCExecutionBefore.Id, "foo", "task1Value");
            runtimeService.SetVariableLocal(userTask2CCExecutionBefore.Id, "foo", "task2Value");

            //var task1Variable = runtimeService.CreateVariableInstanceQuery()
            //    //.VariableValueEquals("foo", "task1Value")
            //    .First();
            //var task2Variable = runtimeService.CreateVariableInstanceQuery()
            //    //.VariableValueEquals("foo", "task2Value")
            //    .First();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the scope variable instance has been overwritten during compaction (conform to prior behavior);
            // although this is tested here, changing this behavior may be ok in the future
            var variables = testHelper.SnapshotAfterMigration.GetVariables();
            Assert.AreEqual(2, variables.Count);

            //var task1VariableAfterMigration = testHelper.SnapshotAfterMigration.GetVariable(task1Variable.Id);
            //Assert.NotNull(task1VariableAfterMigration);
            //Assert.AreEqual("task1Value", task1VariableAfterMigration.Value);

            //var task2VariableAfterMigration = testHelper.SnapshotAfterMigration.GetVariable(task2Variable.Id);
            //Assert.NotNull(task2VariableAfterMigration);
            //Assert.AreEqual("task2Value", task2VariableAfterMigration.Value);
        }

        [Test]
        public virtual void testVariableAtConcurrentExecutionInTransition()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity("userTask")
                .StartBeforeActivity("userTask")
                .Execute();

            var concurrentExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask")
                
                .First();
            var jobForExecution = rule.ManagementService.CreateJobQuery(c=>c.ExecutionId ==concurrentExecution.Id)
                .First();

            runtimeService.SetVariableLocal(concurrentExecution.Id, "var", "value");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var jobAfterMigration = rule.ManagementService.CreateJobQuery(c=>c.Id == jobForExecution.Id)
                .First();

            testHelper.AssertVariableMigratedToExecution(testHelper.SnapshotBeforeMigration.GetSingleVariable("var"),
                jobAfterMigration.ExecutionId);
        }

        [Test]
        public virtual void testVariableAtConcurrentExecutionInTransitionAddParentScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_USER_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity("userTask")
                .StartBeforeActivity("userTask")
                .Execute();

            var concurrentExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask")
                
                .First();
            var jobForExecution = rule.ManagementService.CreateJobQuery(c=>c.ExecutionId ==concurrentExecution.Id)
                .First();

            runtimeService.SetVariableLocal(concurrentExecution.Id, "var", "value");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var jobAfterMigration = rule.ManagementService.CreateJobQuery(c=>c.Id == jobForExecution.Id)
                .First();

            testHelper.AssertVariableMigratedToExecution(testHelper.SnapshotBeforeMigration.GetSingleVariable("var"),
                jobAfterMigration.ExecutionId);
        }

        [Test]
        public virtual void testCanMigrateWithObjectVariableThatFailsOnDeserialization()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var objectValue = ESS.FW.Bpm.Engine.Variable.Variables.SerializedObjectValue("does/not/deserialize")
                .SerializationDataFormat(ESS.FW.Bpm.Engine.Variable.Variables.SerializationDataFormats.Net.ToString())
                //.ObjectTypeName("and.This.Is.a.Nonexisting.Class")
                .Create();

            runtimeService.SetVariable(processInstance.Id, "var", objectValue);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            //IObjectValue migratedValue = runtimeService.GetVariableTyped(processInstance.Id, "var", false);
            //Assert.AreEqual(objectValue.ValueSerialized, migratedValue.ValueSerialized);
            //Assert.AreEqual(objectValue.ObjectTypeName, migratedValue.ObjectTypeName);
        }
    }
}