using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationUserTaskTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationUserTaskTest()
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
        public virtual void testUserTaskMigrationInProcessDefinitionScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then

            // the entities were migrated
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree("userTask")
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Done());

            var task = testHelper.SnapshotBeforeMigration.GetTaskForKey("userTask");
            var migratedTask = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask");
            Assert.AreEqual(task.Id, migratedTask.Id);

            // and it is possible to successfully complete the migrated instance
            rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testUserTaskMigrationInSubProcessScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then

            // the entities were migrated
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("userTask"))
                    .Done());

            var task = testHelper.SnapshotBeforeMigration.GetTaskForKey("userTask");
            var migratedTask = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask");
            Assert.AreEqual(task.Id, migratedTask.Id);
            Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testConcurrentUserTaskMigration()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then

            // the entities were migrated
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("userTask2")
                    .Concurrent()
                    .NoScope()
                    .Done());

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
        public virtual void testCannotMigrateWhenNotAllActivityInstancesAreMapped()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .Build();


            // when
            try
            {
                testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
                Assert.Fail("should not succeed because the userTask2 instance is not mapped");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasActivityInstanceFailures("userTask2",
                        "There is no migration instruction for this instance's activity");
            }
        }

        [Test]
        public virtual void testCannotMigrateWhenNotAllTransitionInstancesAreMapped()
        {
            // given
            IBpmnModelInstance model = ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                //.ActivityBuilder("userTask1")
                //.CamundaAsyncBefore()
                //.MoveToActivity("userTask2")
                //.CamundaAsyncBefore()
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .Build();


            // when
            try
            {
                testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
                Assert.Fail("should not succeed because the userTask2 instance is not mapped");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasTransitionInstanceFailures("userTask2",
                        "There is no migration instruction for this instance's activity");
            }
        }

        [Test]
        public virtual void testChangeActivityId()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask2")
                    .Build();

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity("userTask1")
                .Execute();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree("userTask2")
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("userTask2", testHelper.GetSingleActivityInstanceBeforeMigration("userTask1")
                        .Id)
                    .Done());

            var migratedTask = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask2");
            Assert.NotNull(migratedTask);
            Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);
            Assert.AreEqual("userTask2", migratedTask.TaskDefinitionKey);

            // and it is possible to successfully complete the migrated instance
            rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateWithSubTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var task = rule.TaskService.CreateTaskQuery()
                .First();
            var subTask = rule.TaskService.NewTask();
            subTask.ParentTaskId = task.Id;
            rule.TaskService.SaveTask(subTask);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the sub task properties have not been updated (i.E. subtask should not reference the process instance/definition now)
            var subTaskAfterMigration = rule.TaskService.CreateTaskQuery(c=>c.Id==subTask.Id)
                .First();
            Assert.IsNull(subTaskAfterMigration.ProcessDefinitionId);
            Assert.IsNull(subTaskAfterMigration.ProcessInstanceId);
            Assert.IsNull(subTaskAfterMigration.TaskDefinitionKey);

            // the tasks can be completed and the process can be ended
            rule.TaskService.Complete(subTask.Id);
            rule.TaskService.Complete(task.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);

            if (!rule.ProcessEngineConfiguration.HistoryLevel.Equals(HistoryLevelFields.HistoryLevelNone))
                rule.HistoryService.DeleteHistoricTaskInstance(subTaskAfterMigration.Id);
        }

        [Test]
        public virtual void testAccessModelInTaskListenerAfterMigration()
        {
            IBpmnModelInstance targetModel = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .ChangeElementId("userTask", "newUserTask");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            addTaskListener(targetModel, "newUserTask", TaskListenerFields.EventnameAssignment,
                typeof(AccessModelInstanceTaskListener).FullName);

            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetModel);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "newUserTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // when
            var task = rule.TaskService.CreateTaskQuery()
                .First();

            rule.TaskService.SetAssignee(task.Id, "foo");

            // then the task listener was able to access the bpmn model instance and set a variable
            var variableValue =
                (string)
                rule.RuntimeService.GetVariable(processInstance.Id, AccessModelInstanceTaskListener.VARIABLE_NAME);
            Assert.AreEqual("newUserTask", variableValue);
        }

        protected internal static void addTaskListener(IBpmnModelInstance targetModel, string activityId, string @event,
            string className)
        {
            var taskListener = targetModel.NewInstance<ICamundaTaskListener>(typeof(ICamundaTaskListener));
            taskListener.CamundaClass = className;
            taskListener.CamundaEvent = @event;

            var task = targetModel.GetModelElementById/*<IUserTask>*/(activityId) as IUserTask;
            task.Builder()
                .AddExtensionElement(taskListener);
        }

        public class AccessModelInstanceTaskListener : ITaskListener
        {
            public const string VARIABLE_NAME = "userTaskId";

            public void Notify(IDelegateTask delegateTask)
            {
                var userTask = (IUserTask) delegateTask.BpmnModelElementInstance;
                delegateTask.SetVariable(VARIABLE_NAME, userTask.Id);
            }
        }
    }
}