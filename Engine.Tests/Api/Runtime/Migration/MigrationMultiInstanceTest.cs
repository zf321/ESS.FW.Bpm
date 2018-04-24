using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Migration;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationMultiInstanceTest
    {
        public const string NUMBER_OF_INSTANCES = "nrOfInstances";
        public const string NUMBER_OF_ACTIVE_INSTANCES = "nrOfActiveInstances";
        public const string NUMBER_OF_COMPLETED_INSTANCES = "nrOfCompletedInstances";
        public const string LOOP_COUNTER = "loopCounter";
        private readonly bool InstanceFieldsInitialized;

        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationMultiInstanceTest()
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
        public virtual void testMigrateParallelMultiInstanceTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(miBodyOf("userTask"), miBodyOf("userTask"))
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
                    .Child(null)
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration(miBodyOf("userTask")))
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
                    .Up()
                    .Done());

            var userTaskInstances = testHelper.SnapshotBeforeMigration.ActivityTree.GetActivityInstances("userTask");

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginMiBody("userTask", testHelper.GetSingleActivityInstanceBeforeMigration(miBodyOf("userTask"))
                        .Id)
                    .Activity("userTask", userTaskInstances[0].Id)
                    .Activity("userTask", userTaskInstances[1].Id)
                    .Activity("userTask", userTaskInstances[2].Id)
                    .Done());

            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(3, migratedTasks.Count);
            foreach (var migratedTask in migratedTasks)
                Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateParallelMultiInstanceTasksVariables()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(miBodyOf("userTask"), miBodyOf("userTask"))
                    .MapActivities("userTask", "userTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

            var tasksBeforeMigration = rule.TaskService.CreateTaskQuery()
                
                .ToList();
            IDictionary<string, int?> loopCounterDistribution = new Dictionary<string, int?>();
            foreach (var task in tasksBeforeMigration)
            {
                var loopCounter = (int?) rule.TaskService.GetVariable(task.Id, LOOP_COUNTER);
                loopCounterDistribution[task.Id] = loopCounter;
            }

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var tasks = testHelper.SnapshotAfterMigration.Tasks;
            var firstTask = tasks[0];
            Assert.AreEqual(3, rule.TaskService.GetVariable(firstTask.Id, NUMBER_OF_INSTANCES));
            Assert.AreEqual(3, rule.TaskService.GetVariable(firstTask.Id, NUMBER_OF_ACTIVE_INSTANCES));
            Assert.AreEqual(0, rule.TaskService.GetVariable(firstTask.Id, NUMBER_OF_COMPLETED_INSTANCES));

            foreach (var task in tasks)
            {
                var loopCounter = (int?) rule.TaskService.GetVariable(task.Id, LOOP_COUNTER);
                Assert.NotNull(loopCounter);
                Assert.AreEqual(loopCounterDistribution[task.Id], loopCounter);
            }
        }
        [Test]
        public virtual void testMigrateParallelMultiInstancePartiallyComplete()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(miBodyOf("userTask"), miBodyOf("userTask"))
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteAnyTask("userTask");
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration(miBodyOf("userTask")))
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
                    .Up()
                    .Done());

            var userTaskInstances = testHelper.SnapshotBeforeMigration.ActivityTree.GetActivityInstances("userTask");

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginMiBody("userTask", testHelper.GetSingleActivityInstanceBeforeMigration(miBodyOf("userTask"))
                        .Id)
                    .Activity("userTask", userTaskInstances[0].Id)
                    .Activity("userTask", userTaskInstances[1].Id)
                    .Transition("userTask")
                    .Done()); // bug CAM-5609

            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(2, migratedTasks.Count);
            foreach (var migratedTask in migratedTasks)
                Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }


        [Test]
        public virtual void testMigrateParallelMiBodyRemoveSubprocess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(miBodyOf("subProcess"), miBodyOf("userTask"))
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures(miBodyOf("subProcess"),
                        "Cannot remove the inner activity of a multi-instance body when the body is mapped");
            }
        }


        [Test]
        public virtual void testMigrateParallelMiBodyAddSubprocess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_SUBPROCESS_PROCESS);

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(miBodyOf("userTask"), miBodyOf("subProcess"))
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures(miBodyOf("userTask"),
                        "Must map the inner activity of a multi-instance body when the body is mapped");
            }
        }

        [Test]
        public virtual void testMigrateSequentialMultiInstanceTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(miBodyOf("userTask"), miBodyOf("userTask"))
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
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration(miBodyOf("userTask")))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginMiBody("userTask", testHelper.GetSingleActivityInstanceBeforeMigration(miBodyOf("userTask"))
                        .Id)
                    .Activity("userTask", testHelper.GetSingleActivityInstanceBeforeMigration("userTask")
                        .Id)
                    .Done());

            var migratedTask = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask");
            Assert.NotNull(migratedTask);
            Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.CompleteTask("userTask");
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateSequentialMultiInstanceTasksVariables()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(miBodyOf("userTask"), miBodyOf("userTask"))
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            var task = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask");
            Assert.AreEqual(3, rule.TaskService.GetVariable(task.Id, NUMBER_OF_INSTANCES));
            Assert.AreEqual(1, rule.TaskService.GetVariable(task.Id, NUMBER_OF_ACTIVE_INSTANCES));
            Assert.AreEqual(0, rule.TaskService.GetVariable(task.Id, NUMBER_OF_COMPLETED_INSTANCES));
            Assert.AreEqual(0, rule.TaskService.GetVariable(task.Id, NUMBER_OF_COMPLETED_INSTANCES));
        }

        [Test]
        public virtual void testMigrateSequentialMultiInstancePartiallyComplete()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(miBodyOf("userTask"), miBodyOf("userTask"))
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteAnyTask("userTask");
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration(miBodyOf("userTask")))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginMiBody("userTask", testHelper.GetSingleActivityInstanceBeforeMigration(miBodyOf("userTask"))
                        .Id)
                    .Activity("userTask", testHelper.GetSingleActivityInstanceBeforeMigration("userTask")
                        .Id)
                    .Done());

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }


        [Test]
        public virtual void testMigrateSequenatialMiBodyRemoveSubprocess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(miBodyOf("subProcess"), miBodyOf("userTask"))
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures(miBodyOf("subProcess"),
                        "Cannot remove the inner activity of a multi-instance body when the body is mapped");
            }
        }


        [Test]
        public virtual void testMigrateSequentialMiBodyAddSubprocess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_SUBPROCESS_PROCESS);

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(miBodyOf("userTask"), miBodyOf("subProcess"))
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures(miBodyOf("userTask"),
                        "Must map the inner activity of a multi-instance body when the body is mapped");
            }
        }

        [Test]
        public virtual void testMigrateParallelToSequential()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(miBodyOf("userTask"), miBodyOf("userTask"))
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures(miBodyOf("userTask"),
                        "Activities have incompatible types (ParallelMultiInstanceActivityBehavior is not " +
                        "compatible with SequentialMultiInstanceActivityBehavior)");
            }
        }

        protected internal virtual string miBodyOf(string activityId)
        {
            return activityId + BpmnParse.MultiInstanceBodyIdSuffix;
        }
    }
}