using System;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationTimerCatchEventTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationTimerCatchEventTest()
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
        public virtual void testMigrateJob()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("timerCatch", "timerCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertJobMigrated(testHelper.SnapshotBeforeMigration.Jobs[0], "timerCatch");

            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("timerCatch")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("timerCatch"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("timerCatch", testHelper.GetSingleActivityInstanceBeforeMigration("timerCatch")
                        .Id)
                    .Done());

            // and it is possible to trigger the event
            var jobAfterMigration = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(jobAfterMigration.Id);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateJobChangeActivityId()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        TimerCatchModels.ONE_TIMER_CATCH_PROCESS)
                    .ChangeElementId("timerCatch", "newTimerCatch"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("timerCatch", "newTimerCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertJobMigrated(testHelper.SnapshotBeforeMigration.Jobs[0], "newTimerCatch");

            // and it is possible to trigger the event
            var jobAfterMigration = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(jobAfterMigration.Id);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateJobPreserveTimerConfiguration()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.NewModel()
                .StartEvent()
                .IntermediateCatchEvent("timerCatch")
                .TimerWithDuration("PT50M")
                .UserTask("userTask")
                .EndEvent()
                .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("timerCatch", "timerCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertJobMigrated(testHelper.SnapshotBeforeMigration.Jobs[0], "timerCatch");
            // this also Asserts that the due has not changed

            // and it is possible to trigger the event
            var jobAfterMigration = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(jobAfterMigration.Id);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateJobUpdateTimerConfiguration()
        {
            // given
            ClockTestUtil.SetClockToDateWithoutMilliseconds();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.NewModel()
                .StartEvent()
                .IntermediateCatchEvent("timerCatch")
                .TimerWithDuration("PT50M")
                .UserTask("userTask")
                .EndEvent()
                .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("timerCatch", "timerCatch")
                    .UpdateEventTrigger()
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            var newDueDate = new DateTime(ClockUtil.CurrentTime.Ticks).AddMinutes(50);
            testHelper.AssertJobMigrated(testHelper.SnapshotBeforeMigration.Jobs[0], "timerCatch", newDueDate);

            // and it is possible to trigger the event
            var jobAfterMigration = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(jobAfterMigration.Id);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateJobChangeProcessKey()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        TimerCatchModels.ONE_TIMER_CATCH_PROCESS)
                    .ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("timerCatch", "timerCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertJobMigrated(testHelper.SnapshotBeforeMigration.Jobs[0], "timerCatch");

            // and it is possible to trigger the event
            var jobAfterMigration = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(jobAfterMigration.Id);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateJobAddParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(TimerCatchModels.SUBPROCESS_TIMER_CATCH_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("timerCatch", "timerCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertJobMigrated(testHelper.SnapshotBeforeMigration.Jobs[0], "timerCatch");

            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Scope()
                    .Child("timerCatch")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("timerCatch"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
                    .Activity("timerCatch", testHelper.GetSingleActivityInstanceBeforeMigration("timerCatch")
                        .Id)
                    .Done());

            // and it is possible to trigger the event
            var jobAfterMigration = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(jobAfterMigration.Id);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }
    }
}