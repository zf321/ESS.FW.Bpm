using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationRemoveMultiInstanceTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationRemoveMultiInstanceTest()
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
        public virtual void testRemoveParallelMultiInstanceBody()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
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
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("userTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("userTask")
                    .Concurrent()
                    .NoScope()
                    .Done());

            var userTaskInstances = testHelper.SnapshotBeforeMigration.ActivityTree.GetActivityInstances("userTask");

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("userTask", userTaskInstances[0].Id)
                    .Activity("userTask", userTaskInstances[1].Id)
                    .Activity("userTask", userTaskInstances[2].Id)
                    .Done());

            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(3, migratedTasks.Count);

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }


        [Test]
        public virtual void testRemoveParallelMultiInstanceBodyVariables()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            //Assert.AreEqual(0, rule.RuntimeService.CreateVariableInstanceQuery()
            //    //.VariableName("nrOfInstances")
            //    .Count());

            //// the MI body variables are gone
            //Assert.AreEqual(0, rule.RuntimeService.CreateVariableInstanceQuery()
            //    //.VariableName("nrOfInstances")
            //    .Count());
            //Assert.AreEqual(0, rule.RuntimeService.CreateVariableInstanceQuery()
            //    //.VariableName("nrOfActiveInstances")
            //    .Count());
            //Assert.AreEqual(0, rule.RuntimeService.CreateVariableInstanceQuery()
            //    //.VariableName("nrOfCompletedInstances")
            //    .Count());

            // and the loop counters are still there (because they logically belong to the inner activity instances)
            //Assert.AreEqual(3, rule.RuntimeService.CreateVariableInstanceQuery()
            //    //.VariableName("loopCounter")
            //    .Count());
        }

        [Test]
        public virtual void testRemoveParallelMultiInstanceBodyScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_SUBPROCESS_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            var subProcessInstances = testHelper.SnapshotBeforeMigration.ActivityTree.GetActivityInstances("subProcess");

            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("userTask")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivity(subProcessInstances[0], "subProcess"))
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("userTask")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivity(subProcessInstances[1], "subProcess"))
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("userTask")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivity(subProcessInstances[2], "subProcess"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess", subProcessInstances[0].Id)
                    .Activity("userTask", subProcessInstances[0].GetActivityInstances("userTask")[0].Id)
                    .EndScope()
                    .BeginScope("subProcess", subProcessInstances[1].Id)
                    .Activity("userTask", subProcessInstances[1].GetActivityInstances("userTask")[0].Id)
                    .EndScope()
                    .BeginScope("subProcess", subProcessInstances[2].Id)
                    .Activity("userTask", subProcessInstances[2].GetActivityInstances("userTask")[0].Id)
                    .EndScope()
                    .Done());

            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(3, migratedTasks.Count);

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveParallelMultiInstanceBodyOneInstanceFinished()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

            var firstTask = rule.TaskService.CreateTaskQuery()
                ///*.ListPage(0, 1)*/
                .ToList()
                [0];
            rule.TaskService.Complete(firstTask.Id);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("userTask")
                    .Concurrent()
                    .NoScope()
                    .Done());

            var userTaskInstances = testHelper.SnapshotBeforeMigration.ActivityTree.GetActivityInstances("userTask");

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("userTask", userTaskInstances[0].Id)
                    .Activity("userTask", userTaskInstances[1].Id)
                    .Done());

            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(2, migratedTasks.Count);

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveSequentialMultiInstanceBody()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);
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

            var migratedTask = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask");
            Assert.NotNull(migratedTask);

            // and it is possible to successfully complete the migrated instance
            rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveSequentialMultiInstanceBodyVariables()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then all MI variables are gone
            Assert.AreEqual(0, rule.RuntimeService.CreateVariableInstanceQuery()
                .Count());
        }

        [Test]
        public virtual void testRemovSequentialMultiInstanceBodyScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_SUBPROCESS_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            var subProcessInstance = testHelper.GetSingleActivityInstanceBeforeMigration("subProcess");

            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("subProcess"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess", subProcessInstance.Id)
                    .Activity("userTask", subProcessInstance.GetActivityInstances("userTask")[0].Id)
                    .Done());

            var migratedTask = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask");
            Assert.NotNull(migratedTask);

            // and it is possible to successfully complete the migrated instance
            rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }
    }
}