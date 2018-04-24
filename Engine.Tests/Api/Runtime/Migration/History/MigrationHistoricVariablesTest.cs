using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration.History
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationHistoricVariablesTest
    {
        [SetUp]
        public virtual void initServices()
        {
            runtimeService = rule.RuntimeService;
            taskService = rule.TaskService;
            historyService = rule.HistoryService;
        }

        protected internal static readonly IBpmnModelInstance ONE_BOUNDARY_TASK =
                ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
            /*.ActivityBuilder("userTask").BoundaryEvent().Message("Message").Done()*/;

        protected internal static readonly IBpmnModelInstance CONCURRENT_BOUNDARY_TASKS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
            /*.ActivityBuilder("userTask1").BoundaryEvent().Message("Message").MoveToActivity("userTask2").BoundaryEvent().Message("Message").Done()*/;

        protected internal static readonly IBpmnModelInstance SUBPROCESS_CONCURRENT_BOUNDARY_TASKS =
                ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewaySubprocessProcess)
            /*.ActivityBuilder("userTask1").BoundaryEvent().Message("Message").MoveToActivity("userTask2").BoundaryEvent().Message("Message").Done()*/;

        protected internal IHistoryService historyService;
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal ITaskService taskService;
        protected internal MigrationTestRule testHelper;

        public MigrationHistoricVariablesTest()
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
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        public virtual void noHistoryUpdateOnAddScopeMigration()
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

            // then there is still one historic variable instance
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                .Count());

            // and no additional historic details
            Assert.AreEqual(1, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        public virtual void noHistoryUpdateOnSameStructureMigration()
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

            // then there is still one historic variable instance
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                .Count());

            // and no additional historic details
            Assert.AreEqual(1, historyService.CreateHistoricDetailQuery()
                .Count());
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
        public virtual void testMigrateEventScopeVariable()
        {
            //given
            var sourceDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

            var migrationPlan = rule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                .MapActivities("userTask2", "userTask2")
                .MapActivities("subProcess", "subProcess")
                .MapActivities("compensationBoundary", "compensationBoundary")
                .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceDefinition.Id);

            var subProcessExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask1")
                .First();

            runtimeService.SetVariableLocal(subProcessExecution.Id, "foo", "bar");

            testHelper.CompleteTask("userTask1");

            var eventScopeExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "subProcess")
                .First();
            var eventScopeVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.ExecutionIdIn(eventScopeExecution.Id)
                .First();

            //when
            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var historicVariableInstance = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableId(eventScopeVariable.Id)
                .First();
            Assert.AreEqual(targetDefinition.Id, historicVariableInstance.ProcessDefinitionId);
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
        public virtual void testMigrateHistoryVariableInstance()
        {
            //given
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey));

            var processInstance = runtimeService.StartProcessInstanceById(sourceDefinition.Id);

            runtimeService.SetVariable(processInstance.Id, "test", 3537);
            var instance = historyService.CreateHistoricVariableInstanceQuery()
                .First();

            var migrationPlan = rule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                .MapActivities("userTask", "userTask")
                .Build();

            //when
            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            //then
            var migratedInstance = historyService.CreateHistoricVariableInstanceQuery()
                .First();
            Assert.AreEqual(targetDefinition.Key, migratedInstance.ProcessDefinitionKey);
            Assert.AreEqual(targetDefinition.Id, migratedInstance.ProcessDefinitionId);
            Assert.AreEqual(instance.ActivityInstanceId, migratedInstance.ActivityInstanceId);
            Assert.AreEqual(instance.ExecutionId, migratedInstance.ExecutionId);
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
        public virtual void testMigrateHistoryVariableInstanceMultiInstance()
        {
            //given
            var sourceDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_SUBPROCESS_PROCESS);
            var targetDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_SUBPROCESS_PROCESS);

            var processInstance = runtimeService.StartProcessInstanceById(sourceDefinition.Id);

            var migrationPlan = rule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                .MapEqualActivities()
                .Build();

            //when
            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            //then
            var migratedVariables = historyService.CreateHistoricVariableInstanceQuery()
                
                .ToList();
            Assert.AreEqual(6, migratedVariables.Count);
            // 3 loop counter + nrOfInstance + nrOfActiveInstances + nrOfCompletedInstances

            foreach (var variable in migratedVariables)
            {
                Assert.AreEqual(targetDefinition.Key, variable.ProcessDefinitionKey);
                Assert.AreEqual(targetDefinition.Id, variable.ProcessDefinitionId);
            }
        }
    }
}