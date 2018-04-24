using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Bpmn.ExecutionListener;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationCompensationRemoveSubProcessTest
    {
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        [SetUp]
        [TearDown]
        public virtual void clearExecutionListener()
        {
            RecorderExecutionListener.Clear();
        }

        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationCompensationRemoveSubProcessTest()
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
        public virtual void testCase1()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertEventSubscriptionMigrated("compensationHandler", "compensationHandler", null);

            // and the compensation can be triggered and completed
            testHelper.CompleteTask("userTask2");
            testHelper.CompleteTask("compensationHandler");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCase1AssertActivityInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // when
            testHelper.CompleteTask("userTask2");

            // then
            var activityInstance = rule.RuntimeService.GetActivityInstance(processInstance.Id);

            ActivityInstanceAssert.That(activityInstance)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("compensationEvent")
                    .Activity("compensationHandler")
                    .Done());
        }

        [Test]
        public virtual void testCase1AssertExecutionTree()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree("userTask2")
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Done());
        }

        [Test]
        public virtual void testCase2()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertEventSubscriptionMigrated("compensationHandler", "compensationHandler", null);

            // and the compensation can be triggered and completed
            testHelper.CompleteTask("userTask2");
            testHelper.CompleteTask("compensationHandler");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCase2ActivityInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // when
            testHelper.CompleteTask("userTask2");

            // then
            var activityInstance = rule.RuntimeService.GetActivityInstance(processInstance.Id);

            ActivityInstanceAssert.That(activityInstance)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("compensationEvent")
                    .Activity("compensationHandler")
                    .Done());
        }

        [Test]
        public virtual void testCase2AssertExecutionTree()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree("userTask2")
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Done());
        }

        [Test]
        public virtual void testCanOnlyTriggerCompensationInParentOfRemovedScope()
        {
            var sourceModel = ProcessModels.NewModel()
                .StartEvent()
                .SubProcess("outerSubProcess")
                //.EmbeddedSubProcess()
                //.StartEvent()
                .UserTask("userTask1")
                .BoundaryEvent("compensationBoundary")
                //.CompensateEventDefinition()
                //.CompensateEventDefinitionDone()
                //.MoveToActivity("userTask1")
                .SubProcess("innerSubProcess")
                //.EmbeddedSubProcess()
                //.StartEvent()
                .UserTask("userTask2")
                .EndEvent()
                .SubProcessDone()
                .EndEvent()
                .SubProcessDone()
                .Done();
            CompensationModels.addUserTaskCompensationHandler(sourceModel, "compensationBoundary", "compensationHandler");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceModel);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL)
                        .EndEventBuilder("subProcessEnd")
                        //.CompensateEventDefinition()
                        //.WaitForCompletion(true)
                        //.CompensateEventDefinitionDone()
                        .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("innerSubProcess", "subProcess")
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // when
            testHelper.CompleteTask("userTask2");

            // then compensation is not triggered from inside the inner sub process
            // but only on process definition level
            var activityInstance = rule.RuntimeService.GetActivityInstance(processInstance.Id);

            ActivityInstanceAssert.That(activityInstance)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("compensationEvent")
                    .BeginScope("subProcess")
                    .Activity("compensationHandler")
                    .Done());
        }

        [Test]
        public virtual void testCanRemoveEventScopeWithVariables()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var subProcessExecution = rule.RuntimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask1")
                .First();
            rule.RuntimeService.SetVariableLocal(subProcessExecution.Id, "foo", "bar");

            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            Assert.AreEqual(0, rule.RuntimeService.CreateVariableInstanceQuery()
                .Count());
        }

        [Test]
        public virtual void testDeletesOnlyVariablesFromRemovingScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(CompensationModels.DOUBLE_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("innerSubProcess", "subProcess")
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var innerSubProcessExecution = rule.RuntimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask1")
                .First();

            var outerSubProcessExecutionId = ((ExecutionEntity) innerSubProcessExecution).ParentId;

            rule.RuntimeService.SetVariableLocal(outerSubProcessExecutionId, "outerVariable", "outerValue");
            rule.RuntimeService.SetVariableLocal(innerSubProcessExecution.Id, "innerVariable", "innerValue");

            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);

            var migratedVariable = testHelper.SnapshotAfterMigration.GetSingleVariable("innerVariable");
            Assert.NotNull(migratedVariable);
            Assert.AreEqual("innerValue", migratedVariable.Value);
        }

        [Test]
        public virtual void testNoListenersCalled()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL)
                    //.ActivityBuilder("subProcess")
                    //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    //    typeof(RecorderExecutionListener).FullName)
                    //.Done()
                );
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            // the listener was only called once when the sub process completed properly
            Assert.AreEqual(1, RecorderExecutionListener.RecordedEvents.Count);
        }

        [Test]
        public virtual void testNoOutputMappingExecuted()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL)
                    //.ActivityBuilder("subProcess")
                    //.CamundaOutputParameter("foo", "${bar}")
                    //.Done()
                );
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            rule.RuntimeService.SetVariable(processInstance.Id, "bar", "value1");
            testHelper.CompleteTask("userTask1"); // => sets "foo" to "value1"

            rule.RuntimeService.SetVariable(processInstance.Id, "bar", "value2");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then "foo" has not been set to "value2"
            Assert.AreEqual(2, testHelper.SnapshotAfterMigration.GetVariables()
                .Count); // "foo" and "bar"
            var variableInstance = testHelper.SnapshotAfterMigration.GetSingleVariable("foo");
            Assert.AreEqual("value1", variableInstance.Value);
        }
    }
}