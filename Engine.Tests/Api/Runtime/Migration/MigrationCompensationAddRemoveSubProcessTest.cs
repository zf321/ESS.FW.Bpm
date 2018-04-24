using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationCompensationAddRemoveSubProcessTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationCompensationAddRemoveSubProcessTest()
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
        public virtual void testMigrateCompensationSubscriptionAddRemoveSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

            // subProcess is not mapped
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
        public virtual void testMigrateCompensationSubscriptionAddRemoveSubProcessAssertActivityInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

            // subProcess is not mapped
            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // when compensating
            testHelper.CompleteTask("userTask2");

            // then the activity instance tree is correct
            var activityInstance = rule.RuntimeService.GetActivityInstance(processInstance.Id);

            ActivityInstanceAssert.That(activityInstance)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("compensationEvent")
                    .BeginScope("subProcess")
                    .Activity("compensationHandler")
                    .Done());
        }

        [Test]
        public virtual void testMigrateCompensationSubscriptionAddRemoveSubProcessAssertExecutionTree()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

            // subProcess is not mapped
            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the execution tree is correct
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree("userTask2")
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("subProcess")
                    .Scope()
                    .EventScope()
                    .Done());
        }
    }
}