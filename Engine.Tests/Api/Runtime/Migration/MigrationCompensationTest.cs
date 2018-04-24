using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationCompensationTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationCompensationTest()
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

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        [Test]
        public virtual void testCannotMigrateActivityInstanceForCompensationThrowingEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("compensationEvent", "compensationEvent")
                    .MapActivities("compensationHandler", "compensationHandler")
                    .Build();

            // when
            try
            {
                testHelper.MigrateProcessInstance(migrationPlan, processInstance);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasProcessInstanceId(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .HasActivityInstanceFailures("compensationEvent",
                        "The type of the source activity is not supported for activity instance migration");
            }
        }

        [Test]
        public virtual void testCannotMigrateActivityInstanceForCancelEndEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.TRANSACTION_COMPENSATION_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.TRANSACTION_COMPENSATION_MODEL);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask");

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("transactionEndEvent", "transactionEndEvent")
                    .MapActivities("compensationHandler", "compensationHandler")
                    .Build();

            // when
            try
            {
                testHelper.MigrateProcessInstance(migrationPlan, processInstance);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasProcessInstanceId(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .HasActivityInstanceFailures("transactionEndEvent",
                        "The type of the source activity is not supported for activity instance migration");
            }
        }

        [Test]
        public virtual void testCannotMigrateActiveCompensationWithoutInstructionForThrowingEventCase1()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("compensationHandler", "compensationHandler")
                    .Build();

            // when
            try
            {
                testHelper.MigrateProcessInstance(migrationPlan, processInstance);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasProcessInstanceId(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .HasActivityInstanceFailures("compensationEvent",
                        "There is no migration instruction for this instance's activity",
                        "The type of the source activity is not supported for activity instance migration");
            }
        }

        [Test]
        public virtual void testCannotMigrateActiveCompensationWithoutInstructionForThrowingEventCase2()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_END_EVENT_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_END_EVENT_MODEL);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("compensationHandler", "compensationHandler")
                    .Build();

            // when
            try
            {
                testHelper.MigrateProcessInstance(migrationPlan, processInstance);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasProcessInstanceId(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .HasActivityInstanceFailures("compensationEvent",
                        "There is no migration instruction for this instance's activity",
                        "The type of the source activity is not supported for activity instance migration");
            }
        }

        [Test]
        public virtual void testCannotMigrateWithoutMappingCompensationBoundaryEvents()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            try
            {
                testHelper.MigrateProcessInstance(migrationPlan, processInstance);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasProcessInstanceId(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .HasActivityInstanceFailures(sourceProcessDefinition.Id,
                        "Cannot migrate subscription for compensation handler 'compensationHandler'. " +
                        "There is no migration instruction for the compensation boundary event");
            }
        }

        [Test]
        public virtual void testCannotRemoveCompensationEventSubscriptions()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            try
            {
                testHelper.MigrateProcessInstance(migrationPlan, processInstance);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasProcessInstanceId(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .HasActivityInstanceFailures(sourceProcessDefinition.Id,
                        "Cannot migrate subscription for compensation handler 'compensationHandler'. " +
                        "There is no migration instruction for the compensation boundary event");
            }
        }

        [Test]
        public virtual void testCanRemoveCompensationBoundaryWithoutEventSubscriptions()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);
            testHelper.CompleteTask("userTask1");

            // then
            Assert.AreEqual(0, testHelper.SnapshotAfterMigration.EventSubscriptions.Count);

            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCannotTriggerAddedCompensationForCompletedInstances()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            Assert.AreEqual(0, testHelper.SnapshotAfterMigration.EventSubscriptions.Count);

            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCanTriggerAddedCompensationForActiveInstances()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask1")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.CompleteTask("userTask1");
            Assert.AreEqual(1, rule.RuntimeService.CreateEventSubscriptionQuery()
                .Count());

            testHelper.CompleteTask("userTask2");
            testHelper.CompleteTask("compensationHandler");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCanMigrateWithCompensationSubscriptionsInMigratingScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
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
        public virtual void testCanMigrateWithCompensationSubscriptionsInMigratingScopeAssertActivityInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // a migrated process instance
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // when triggering compensation
            testHelper.CompleteTask("userTask2");

            // then the activity instance tree is correct
            var activityInstance = rule.RuntimeService.GetActivityInstance(processInstance.Id);

            ActivityInstanceAssert.That(activityInstance)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("compensationEvent")
                    .Activity("compensationHandler")
                    .Done());
        }

        [Test]
        public virtual void testCanMigrateWithCompensationSubscriptionsInMigratingScopeAssertExecutionTree()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
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
        public virtual void testCanMigrateWithCompensationSubscriptionsInMigratingScopeChangeIds()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        CompensationModels.ONE_COMPENSATION_TASK_MODEL)
                    .ChangeElementId("userTask1", "newUserTask1")
                    .ChangeElementId("compensationBoundary", "newCompensationBoundary")
                    .ChangeElementId("compensationHandler", "newCompensationHandler"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "newCompensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertEventSubscriptionMigrated("compensationHandler", "newCompensationHandler", null);

            // and the compensation can be triggered and completed
            testHelper.CompleteTask("userTask2");
            testHelper.CompleteTask("newCompensationHandler");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCanMigrateWithCompensationEventScopeExecution()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertEventSubscriptionMigrated("subProcess", "subProcess", null);
            testHelper.AssertEventSubscriptionMigrated("compensationHandler", "compensationHandler", null);

            // and the compensation can be triggered and completed
            testHelper.CompleteTask("userTask2");
            testHelper.CompleteTask("compensationHandler");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCanMigrateWithCompensationEventScopeExecutionAssertActivityInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
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
                    .BeginScope("subProcess")
                    .Activity("compensationHandler")
                    .Done());
        }

        [Test]
        public virtual void testCanMigrateWithCompensationEventScopeExecutionAssertExecutionTree()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            var eventScopeExecution = rule.RuntimeService.CreateExecutionQuery(c=>c.ActivityId == "subProcess")
                .First();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree("userTask2")
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("subProcess")
                    .Scope()
                    .EventScope()
                    .Id(eventScopeExecution.Id)
                    .Done());
        }

        [Test]
        public virtual void testCanMigrateWithCompensationEventScopeExecutionChangeIds()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL)
                    .ChangeElementId("subProcess", "newSubProcess")
                    .ChangeElementId("userTask1", "newUserTask1")
                    .ChangeElementId("compensationBoundary", "newCompensationBoundary")
                    .ChangeElementId("compensationHandler", "newCompensationHandler"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("subProcess", "newSubProcess")
                    .MapActivities("compensationBoundary", "newCompensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertEventSubscriptionMigrated("subProcess", "newSubProcess", null);
            testHelper.AssertEventSubscriptionMigrated("compensationHandler", "newCompensationHandler", null);

            // and the compensation can be triggered and completed
            testHelper.CompleteTask("userTask2");
            testHelper.CompleteTask("newCompensationHandler");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCanMigrateWithCompensationEventScopeExecutionChangeIdsAssertActivityInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL)
                    .ChangeElementId("subProcess", "newSubProcess")
                    .ChangeElementId("userTask1", "newUserTask1")
                    .ChangeElementId("compensationBoundary", "newCompensationBoundary")
                    .ChangeElementId("compensationHandler", "newCompensationHandler"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("subProcess", "newSubProcess")
                    .MapActivities("compensationBoundary", "newCompensationBoundary")
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
                    .BeginScope("newSubProcess")
                    .Activity("newCompensationHandler")
                    .Done());
        }

        [Test]
        public virtual void testCanMigrateWithCompensationEventScopeExecutionChangeIdsAssertExecutionTree()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL)
                    .ChangeElementId("subProcess", "newSubProcess")
                    .ChangeElementId("userTask1", "newUserTask1")
                    .ChangeElementId("compensationBoundary", "newCompensationBoundary")
                    .ChangeElementId("compensationHandler", "newCompensationHandler"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("subProcess", "newSubProcess")
                    .MapActivities("compensationBoundary", "newCompensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            var eventScopeExecution = rule.RuntimeService.CreateExecutionQuery(c=>c.ActivityId == "subProcess")
                .First();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree("userTask2")
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("newSubProcess")
                    .Scope()
                    .EventScope()
                    .Id(eventScopeExecution.Id)
                    .Done());
        }

        [Test]
        public virtual void testCanMigrateEventScopeVariables()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
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
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("foo");
            testHelper.AssertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);

            // and the compensation can be triggered and completed
            testHelper.CompleteTask("userTask2");
            testHelper.CompleteTask("compensationHandler");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCanMigrateWithEventSubProcessHandler()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("eventSubProcessStart", "eventSubProcessStart")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertEventSubscriptionMigrated("eventSubProcess", "eventSubProcess", null);

            // and the compensation can be triggered and completed
            testHelper.CompleteTask("userTask2");
            testHelper.CompleteTask("eventSubProcessTask");
            testHelper.CompleteTask("compensationHandler");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCanMigrateWithEventSubProcessHandlerAssertActivityInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("eventSubProcessStart", "eventSubProcessStart")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteTask("userTask1");
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // when compensation is triggered
            testHelper.CompleteTask("userTask2");

            // then
            var activityInstance = rule.RuntimeService.GetActivityInstance(processInstance.Id);

            ActivityInstanceAssert.That(activityInstance)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("compensationEvent")
                    .BeginScope("subProcess")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());
        }

        [Test]
        public virtual void testCanMigrateWithEventSubProcessHandlerAssertExecutionTree()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("eventSubProcessStart", "eventSubProcessStart")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteTask("userTask1");

            var eventScopeExecution = rule.RuntimeService.CreateExecutionQuery(c=>c.ActivityId == "subProcess")
                .First();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree("userTask2")
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("subProcess")
                    .Scope()
                    .EventScope()
                    .Id(eventScopeExecution.Id)
                    .Done());
        }

        [Test]
        public virtual void testCanMigrateWithEventSubProcessHandlerChangeIds()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL)
                    .ChangeElementId("eventSubProcess", "newEventSubProcess"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("eventSubProcessStart", "eventSubProcessStart")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertEventSubscriptionMigrated("eventSubProcess", "newEventSubProcess", null);

            // and the compensation can be triggered and completed
            testHelper.CompleteTask("userTask2");
            testHelper.CompleteTask("eventSubProcessTask");
            testHelper.CompleteTask("compensationHandler");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCanMigrateSiblingEventScopeExecutions()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(CompensationModels.DOUBLE_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("subProcess", "outerSubProcess")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            // starting a second instances of the sub process
            rule.RuntimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("subProcess")
                .Execute();

            var subProcessExecutions = rule.RuntimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask1")
                
                .ToList();
            foreach (var subProcessExecution in subProcessExecutions)
                rule.RuntimeService.SetVariableLocal(subProcessExecution.Id, "var", subProcessExecution.Id);

            testHelper.CompleteAnyTask("userTask1");
            testHelper.CompleteAnyTask("userTask1");


            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the variable snapshots during compensation are not shared
            testHelper.CompleteAnyTask("userTask2");

            var compensationTasks = rule.TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "compensationHandler")
                
                .ToList();
            Assert.AreEqual(2, compensationTasks.Count);

            var value1 = rule.TaskService.GetVariable(compensationTasks[0].Id, "var");
            var value2 = rule.TaskService.GetVariable(compensationTasks[1].Id, "var");
            Assert.AreNotEqual(value1, value2);
        }

        [Test]
        public virtual void testCannotMigrateWithoutCompensationStartEventCase1()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            // when
            try
            {
                testHelper.MigrateProcessInstance(migrationPlan, processInstance);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasProcessInstanceId(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .HasActivityInstanceFailures(sourceProcessDefinition.Id,
                        "Cannot migrate subscription for compensation handler 'eventSubProcess'. " +
                        "There is no migration instruction for the compensation start event");
            }
        }

        [Test]
        public virtual void testCannotMigrateWithoutCompensationStartEventCase2()
        {
            // given
            IBpmnModelInstance model = ModifiableBpmnModelInstance.Modify(
                    CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL)
                .RemoveFlowNode("compensationBoundary");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            try
            {
                testHelper.MigrateProcessInstance(migrationPlan, processInstance);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasProcessInstanceId(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .HasActivityInstanceFailures(sourceProcessDefinition.Id,
                        "Cannot migrate subscription for compensation handler 'eventSubProcess'. " +
                        "There is no migration instruction for the compensation start event");
            }
        }

        [Test]
        public virtual void testEventScopeHierarchyPreservation()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(CompensationModels.DOUBLE_SUBPROCESS_MODEL);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(CompensationModels.DOUBLE_SUBPROCESS_MODEL);

            try
            {
                // when
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("outerSubProcess", "innerSubProcess")
                    .MapActivities("innerSubProcess", "outerSubProcess")
                    .Build();
                Assert.Fail("exception expected");
            }
            catch (MigrationPlanValidationException e)
            {
                // then
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("innerSubProcess",
                        "The closest mapped ancestor 'outerSubProcess' is mapped to scope 'innerSubProcess' " +
                        "which is not an ancestor of target scope 'outerSubProcess'");
            }
        }

        [Test]
        public virtual void testCompensationBoundaryHierarchyPreservation()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL)
                        .AddSubProcessTo(ProcessModels.ProcessKey)
                        //.Id("addedSubProcess")
                        ////.EmbeddedSubProcess()
                        //.StartEvent()
                        .EndEvent()
                        .Done());

            try
            {
                // when
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "addedSubProcess")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();
                Assert.Fail("exception expected");
            }
            catch (MigrationPlanValidationException e)
            {
                // then
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("compensationBoundary",
                        "The closest mapped ancestor 'subProcess' is mapped to scope 'addedSubProcess' " +
                        "which is not an ancestor of target scope 'compensationBoundary'");
            }
        }

        [Test]
        public virtual void testCannotMapCompensateStartEventWithoutMappingEventScopeCase1()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);

            try
            {
                // when
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventSubProcessStart", "eventSubProcessStart")
                    .Build();
                Assert.Fail("exception expected");
            }
            catch (MigrationPlanValidationException e)
            {
                // then
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("eventSubProcessStart",
                        "The source activity's event scope (subProcess) must be mapped to the target activity's event scope (subProcess)");
            }
        }

        [Test]
        public virtual void testCannotMapCompensateStartEventWithoutMappingEventScopeCase2()
        {
            // given
            IBpmnModelInstance model = ModifiableBpmnModelInstance.Modify(
                    CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL)
                .RemoveFlowNode("compensationBoundary");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            try
            {
                // when
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventSubProcessStart", "eventSubProcessStart")
                    .Build();
                Assert.Fail("exception expected");
            }
            catch (MigrationPlanValidationException e)
            {
                // then
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("eventSubProcessStart",
                        "The source activity's event scope (subProcess) must be mapped to the target activity's event scope (subProcess)");
            }
        }
    }
}