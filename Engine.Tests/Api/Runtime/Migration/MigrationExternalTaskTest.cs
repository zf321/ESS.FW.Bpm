using System;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationExternalTaskTest
    {
        public const string WORKER_ID = "foo";
        private readonly bool InstanceFieldsInitialized;

        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationExternalTaskTest()
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
        public virtual void testTrees()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("externalTask", "externalTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the execution and activity instance tree are exactly as before migration
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("externalTask")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("externalTask"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("externalTask", testHelper.GetSingleActivityInstanceBeforeMigration("externalTask")
                        .Id)
                    .Done());
        }

        [Test]
        public virtual void testProperties()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS)
                    .ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey)
                    .ChangeElementId("externalTask", "newExternalTask"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("externalTask", "newExternalTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var externalTaskBeforeMigration = rule.ExternalTaskService.CreateExternalTaskQuery()
                .First();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then all properties are the same apart from the process reference
            var externalTaskAfterMigration = rule.ExternalTaskService.CreateExternalTaskQuery()
                .First();

            Assert.AreEqual("newExternalTask", externalTaskAfterMigration.ActivityId);
            Assert.AreEqual(targetProcessDefinition.Id, externalTaskAfterMigration.ProcessDefinitionId);
            Assert.AreEqual("new" + ProcessModels.ProcessKey, externalTaskAfterMigration.ProcessDefinitionKey);

            Assert.AreEqual(externalTaskBeforeMigration.Priority, externalTaskAfterMigration.Priority);
            Assert.AreEqual(externalTaskBeforeMigration.ActivityInstanceId,
                externalTaskAfterMigration.ActivityInstanceId);
            Assert.AreEqual(externalTaskBeforeMigration.ErrorMessage, externalTaskAfterMigration.ErrorMessage);
            Assert.AreEqual(externalTaskBeforeMigration.ExecutionId, externalTaskAfterMigration.ExecutionId);
            Assert.AreEqual(externalTaskBeforeMigration.Id, externalTaskAfterMigration.Id);
            Assert.AreEqual(externalTaskBeforeMigration.LockExpirationTime,
                externalTaskAfterMigration.LockExpirationTime);
            Assert.AreEqual(processInstance.Id, externalTaskAfterMigration.ProcessInstanceId);
            Assert.AreEqual(externalTaskBeforeMigration.Retries, externalTaskAfterMigration.Retries);
            Assert.AreEqual(externalTaskBeforeMigration.TenantId, externalTaskAfterMigration.TenantId);
            Assert.AreEqual(externalTaskBeforeMigration.TopicName, externalTaskAfterMigration.TopicName);
            Assert.AreEqual(externalTaskBeforeMigration.WorkerId, externalTaskAfterMigration.WorkerId);
        }


        [Test]
        public virtual void testContinueProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("externalTask", "externalTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then it is possible to complete the task
            var task = fetchAndLockSingleTask(ExternalTaskModels.TOPIC);
            rule.ExternalTaskService.Complete(task.Id, WORKER_ID);

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testChangeTaskConfiguration()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS)
                        .ServiceTaskBuilder("externalTask")
                        .CamundaTopic("new" + ExternalTaskModels.TOPIC)
                        .CamundaTaskPriority(Convert.ToString(ExternalTaskModels.PRIORITY * 2))
                        .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("externalTask", "externalTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the task's topic and priority have not changed
            var externalTaskAfterMigration = rule.ExternalTaskService.CreateExternalTaskQuery()
                .First();
            Assert.AreEqual(ExternalTaskModels.PRIORITY, externalTaskAfterMigration.Priority);
            Assert.AreEqual(ExternalTaskModels.TOPIC, externalTaskAfterMigration.TopicName);
        }

        [Test]
        public virtual void testChangeTaskType()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.NewModel()
                .StartEvent()
                .BusinessRuleTask("externalBusinessRuleTask")
                .CamundaType(ExternalTaskModels.EXTERNAL_TASK_TYPE)
                .CamundaTopic(ExternalTaskModels.TOPIC)
                .CamundaTaskPriority(ExternalTaskModels.PRIORITY.ToString())
                .EndEvent()
                .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("externalTask", "externalBusinessRuleTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the task and process can be completed
            var task = fetchAndLockSingleTask(ExternalTaskModels.TOPIC);
            rule.ExternalTaskService.Complete(task.Id, WORKER_ID);

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testLockedTaskProperties()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS)
                    .ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey)
                    .ChangeElementId("externalTask", "newExternalTask"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("externalTask", "newExternalTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            fetchAndLockSingleTask(ExternalTaskModels.TOPIC);
            var externalTaskBeforeMigration = rule.ExternalTaskService.CreateExternalTaskQuery()
                .First();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the locking properties have not been changed
            var externalTaskAfterMigration = rule.ExternalTaskService.CreateExternalTaskQuery()
                .First();

            Assert.AreEqual(externalTaskBeforeMigration.LockExpirationTime,
                externalTaskAfterMigration.LockExpirationTime);
            Assert.AreEqual(externalTaskBeforeMigration.WorkerId, externalTaskAfterMigration.WorkerId);
        }

        [Test]
        public virtual void testLockedTaskContinueProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS)
                    .ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey)
                    .ChangeElementId("externalTask", "newExternalTask"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("externalTask", "newExternalTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var externalTask = fetchAndLockSingleTask(ExternalTaskModels.TOPIC);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then it is possible to complete the task and the process
            rule.ExternalTaskService.Complete(externalTask.Id, WORKER_ID);

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void cannotMigrateFromExternalToClassDelegateServiceTask()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ServiceTaskModels.oneClassDelegateServiceTask("foo.Bar"));

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("externalTask", "serviceTask")
                    .Build();
                Assert.Fail("exception expected");
            }
            catch (MigrationPlanValidationException e)
            {
                // then
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("externalTask",
                        "Activities have incompatible types (ExternalTaskActivityBehavior is not compatible with" +
                        " ClassDelegateActivityBehavior)");
            }
        }

        [Test]
        public virtual void testAddParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.SUBPROCESS_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("externalTask", "externalTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then it is possible to complete the task
            var task = fetchAndLockSingleTask(ExternalTaskModels.TOPIC);
            rule.ExternalTaskService.Complete(task.Id, WORKER_ID);

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testRemoveParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.SUBPROCESS_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("externalTask", "externalTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then it is possible to complete the task
            var task = fetchAndLockSingleTask(ExternalTaskModels.TOPIC);
            rule.ExternalTaskService.Complete(task.Id, WORKER_ID);

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testIncident()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS)
                    .ChangeElementId("externalTask", "newExternalTask"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("externalTask", "newExternalTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var externalTask = rule.ExternalTaskService.CreateExternalTaskQuery()
                .First();
            rule.ExternalTaskService.SetRetries(externalTask.Id, 0);

            var incidentBeforeMigration = rule.RuntimeService.CreateIncidentQuery()
                .First();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the incident has migrated
            var incidentAfterMigration = rule.RuntimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(incidentAfterMigration);

            Assert.AreEqual(incidentBeforeMigration.Id, incidentAfterMigration.Id);
            Assert.AreEqual(IncidentFields.ExternalTaskHandlerType, incidentAfterMigration.IncidentType);
            Assert.AreEqual(externalTask.Id, incidentAfterMigration.Configuration);

            Assert.AreEqual("newExternalTask", incidentAfterMigration.ActivityId);
            Assert.AreEqual(targetProcessDefinition.Id, incidentAfterMigration.ProcessDefinitionId);
            Assert.AreEqual(externalTask.ExecutionId, incidentAfterMigration.ExecutionId);

            // and it is possible to complete the process
            rule.ExternalTaskService.SetRetries(externalTask.Id, 1);

            var task = fetchAndLockSingleTask(ExternalTaskModels.TOPIC);
            rule.ExternalTaskService.Complete(task.Id, WORKER_ID);

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testIncidentWithoutMapExternalTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS)
                    .ChangeElementId("externalTask", "newExternalTask"));

            //external task is not mapped to new external task
            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var externalTask = rule.ExternalTaskService.CreateExternalTaskQuery()
                .First();
            rule.ExternalTaskService.SetRetries(externalTask.Id, 0);

            var incidentBeforeMigration = rule.RuntimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(incidentBeforeMigration);

            // when migration is executed
            try
            {
                testHelper.MigrateProcessInstance(migrationPlan, processInstance);
                Assert.Fail("Exception expected!");
            }
            catch (System.Exception ex)
            {
                Assert.True(ex is MigratingProcessInstanceValidationException);
            }
        }

        protected internal virtual ILockedExternalTask fetchAndLockSingleTask(string topic)
        {
            //var tasks = rule.ExternalTaskService.FetchAndLock(1, WORKER_ID)
            //    //.Topic(topic, 1000L)
            //    .Execute();

            //Assert.AreEqual(1, tasks.Count);

            //return tasks[0];
            return null;
        }
    }
}