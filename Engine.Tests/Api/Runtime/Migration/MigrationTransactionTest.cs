using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationTransactionTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationTransactionTest()
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
        public virtual void testContinueProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("transaction", "transaction")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testContinueProcessTriggerCancellation()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.CANCEL_BOUNDARY_EVENT);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("transaction", "transaction")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.CompleteTask("userTask");
            testHelper.CompleteTask("afterBoundaryTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testAssertTrees()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("transaction", "transaction")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("userTask"))
                    .Up()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("transaction", testHelper.GetSingleActivityInstanceBeforeMigration("transaction")
                        .Id)
                    .Activity("userTask", testHelper.GetSingleActivityInstanceBeforeMigration("userTask")
                        .Id)
                    .Done());
        }

        [Test]
        public virtual void testAddTransactionContinueProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testAddTransactionTriggerCancellation()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.CANCEL_BOUNDARY_EVENT);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.CompleteTask("userTask");
            testHelper.CompleteTask("afterBoundaryTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testAddTransactionAssertTrees()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("transaction")
                    .Activity("userTask", testHelper.GetSingleActivityInstanceBeforeMigration("userTask")
                        .Id)
                    .Done());
        }

        [Test]
        public virtual void testRemoveTransactionContinueProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testRemoveTransactionAssertTrees()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree("userTask")
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("userTask", testHelper.GetSingleActivityInstanceBeforeMigration("userTask")
                        .Id)
                    .Done());
        }
        [Test]
        public virtual void testMigrateTransactionToEmbeddedSubProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("transaction", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            Assert.AreEqual(testHelper.GetSingleActivityInstanceBeforeMigration("transaction")
                .Id, testHelper.GetSingleActivityInstanceAfterMigration("subProcess")
                .Id);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubProcessToTransaction()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(TransactionModels.ONE_TASK_TRANSACTION);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventSubProcess", "transaction")
                    .MapActivities("eventSubProcessTask", "userTask")
                    .Build();

            // when
            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity("eventSubProcessTask")
                .Execute();

            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            Assert.AreEqual(testHelper.GetSingleActivityInstanceBeforeMigration("eventSubProcess")
                .Id, testHelper.GetSingleActivityInstanceAfterMigration("transaction")
                .Id);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }
    }
}