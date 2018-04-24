using System.Collections.Generic;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationSignalCatchEventTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationSignalCatchEventTest()
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
        public virtual void testMigrateEventSubscription()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("signalCatch", "signalCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated("signalCatch", "signalCatch", SignalCatchModels.SIGNAL_NAME);

            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("signalCatch")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("signalCatch"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("signalCatch", testHelper.GetSingleActivityInstanceBeforeMigration("signalCatch")
                        .Id)
                    .Done());

            // and it is possible to trigger the event
            rule.RuntimeService.SignalEventReceived(SignalCatchModels.SIGNAL_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubscriptionChangeActivityId()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS)
                    .ChangeElementId("signalCatch", "newSignalCatch"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("signalCatch", "newSignalCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated("signalCatch", "newSignalCatch", SignalCatchModels.SIGNAL_NAME);

            // and it is possible to trigger the event
            rule.RuntimeService.SignalEventReceived(SignalCatchModels.SIGNAL_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubscriptionPreserveSignalName()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.NewModel()
                .StartEvent()
                .IntermediateCatchEvent("signalCatch")
                .Signal("new" + SignalCatchModels.SIGNAL_NAME)
                .UserTask("userTask")
                .EndEvent()
                .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("signalCatch", "signalCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then the signal name of the event subscription has not changed
            testHelper.AssertEventSubscriptionMigrated("signalCatch", "signalCatch", SignalCatchModels.SIGNAL_NAME);

            // and it is possible to trigger the event
            rule.RuntimeService.SignalEventReceived(SignalCatchModels.SIGNAL_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubscriptionUpdateSignalName()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.NewModel()
                .StartEvent()
                .IntermediateCatchEvent("signalCatch")
                .Signal("new" + SignalCatchModels.SIGNAL_NAME)
                .UserTask("userTask")
                .EndEvent()
                .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("signalCatch", "signalCatch")
                    .UpdateEventTrigger()
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then the message event subscription's event name has not changed
            testHelper.AssertEventSubscriptionMigrated("signalCatch", SignalCatchModels.SIGNAL_NAME, "signalCatch",
                "new" + SignalCatchModels.SIGNAL_NAME);

            // and it is possible to trigger the event
            rule.RuntimeService.SignalEventReceived("new" + SignalCatchModels.SIGNAL_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateJobAddParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(SignalCatchModels.SUBPROCESS_SIGNAL_CATCH_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("signalCatch", "signalCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated("signalCatch", "signalCatch", SignalCatchModels.SIGNAL_NAME);

            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Scope()
                    .Child("signalCatch")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("signalCatch"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
                    .Activity("signalCatch", testHelper.GetSingleActivityInstanceBeforeMigration("signalCatch")
                        .Id)
                    .Done());

            // and it is possible to trigger the event
            rule.RuntimeService.SignalEventReceived(SignalCatchModels.SIGNAL_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubscriptionUpdateSignalExpressionNameWithVariables()
        {
            // given
            var newSignalName = "new" + SignalCatchModels.SIGNAL_NAME + "-${var}";
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(SignalCatchModels.ONE_SIGNAL_CATCH_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.NewModel()
                .StartEvent()
                .IntermediateCatchEvent("signalCatch")
                .Signal(newSignalName)
                .UserTask("userTask")
                .EndEvent()
                .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("signalCatch", "signalCatch")
                    .UpdateEventTrigger()
                    .Build();

            var variables = new Dictionary<string, object>();
            variables["var"] = "foo";


            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan, variables);

            // then there should be a variable
            var beforeMigration = testHelper.SnapshotBeforeMigration.GetSingleVariable("var");
            Assert.AreEqual(1, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
            testHelper.AssertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);

            // and the signal event subscription's event name has changed
            var resolvedSignalName = "new" + SignalCatchModels.SIGNAL_NAME + "-foo";
            testHelper.AssertEventSubscriptionMigrated("signalCatch", SignalCatchModels.SIGNAL_NAME, "signalCatch",
                resolvedSignalName);

            // and it is possible to trigger the event and complete the task afterwards
            rule.RuntimeService.SignalEventReceived(resolvedSignalName);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }
    }
}