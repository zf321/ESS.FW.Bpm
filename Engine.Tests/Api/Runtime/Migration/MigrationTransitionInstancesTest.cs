using System.Linq;
using Engine.Tests.Api.Mgmt;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationTransitionInstancesTest
    {
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        [TearDown]
        public virtual void resetClock()
        {
            ClockUtil.Reset();
        }

        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationTransitionInstancesTest()
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
        public virtual void testMigrateAsyncBeforeTransitionInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
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
                    .Transition("userTask")
                    .Done());

            testHelper.AssertJobMigrated("userTask", "userTask", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncBeforeTransitionInstanceChangeActivityId()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS)
                    .ChangeElementId("userTask", "userTaskReplacement"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTaskReplacement")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertJobMigrated("userTask", "userTaskReplacement", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteTask("userTaskReplacement");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncBeforeTransitionInstanceConcurrent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(migrationPlan.SourceProcessDefinitionId)
                .StartBeforeActivity("userTask")
                .StartBeforeActivity("userTask")
                .Execute();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var transitionInstances = testHelper.SnapshotAfterMigration.ActivityTree.GetTransitionInstances("userTask");

            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask")
                    .Concurrent()
                    .NoScope()
                    .Id(transitionInstances[0].ExecutionId)
                    .Up()
                    .Child("userTask")
                    .Concurrent()
                    .NoScope()
                    .Id(transitionInstances[1].ExecutionId)
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Transition("userTask")
                    .Transition("userTask")
                    .Done());

            Assert.AreEqual(2, testHelper.SnapshotAfterMigration.Jobs.Count);

            // and it is possible to successfully execute the migrated job
            foreach (var job in testHelper.SnapshotAfterMigration.Jobs)
            {
                rule.ManagementService.ExecuteJob(job.Id);
                testHelper.CompleteTask("userTask");
            }

            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncAfterTransitionInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertJobMigrated("userTask1", "userTask1", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }
        [Test]
        public virtual void testMigrateAsyncAfterTransitionInstanceChangeActivityId()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS)
                    .ChangeElementId("userTask1", "userTaskReplacement1")
                    .ChangeElementId("userTask2", "userTaskReplacement2"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTaskReplacement1")
                    .MapActivities("userTask2", "userTaskReplacement2")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertJobMigrated("userTask1", "userTaskReplacement1", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteTask("userTaskReplacement2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncBeforeTransitionInstanceRemoveIncomingFlow()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS)
                    .RemoveFlowNode("startEvent"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertJobMigrated("userTask", "userTask", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncBeforeTransitionInstanceAddIncomingFlow()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS)
                    .RemoveFlowNode("startEvent"));
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity("userTask")
                .Execute();

            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertJobMigrated("userTask", "userTask", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncAfterTransitionInstanceRemoveOutgoingFlowCase1()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS)
                    .RemoveFlowNode("endEvent")
                    .RemoveFlowNode("userTask2"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertJobMigrated("userTask1", "userTask1", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncAfterTransitionInstanceRemoveOutgoingFlowCase2()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_SUBPROCESS_USER_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertJobMigrated("userTask1", "userTask1", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }


        [Test]
        public virtual void testMigrateAsyncAfterTransitionInstanceAddOutgoingFlowCase1()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS)
                    .RemoveFlowNode("endEvent")
                    .RemoveFlowNode("userTask2"));
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertJobMigrated("userTask1", "userTask1", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the process instance
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncAfterTransitionInstanceAddOutgoingFlowCase2()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS)
                    //.ActivityBuilder("userTask1")
                    //.UserTask("userTask3")
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertJobMigrated("userTask1", "userTask1", AsyncContinuationJobHandler.TYPE);


            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the process instance
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncAfterTransitionInstanceAddOutgoingFlowCase3()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS)
                    //.ChangeElementId("flow1", "flow2")
                    //.ActivityBuilder("userTask1")
                    //.SequenceFlowId("flow3")
                    //.UserTask("userTask3")
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            try
            {
                testHelper.MigrateProcessInstance(migrationPlan, processInstance);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasTransitionInstanceFailures("userTask1",
                        "Transition instance is assigned to a sequence flow that cannot be matched in the target activity");
            }
        }

        [Test]
        public virtual void testMigrateAsyncAfterTransitionInstanceReplaceOutgoingFlow()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS)
                    .ChangeElementId("flow1", "flow2"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertJobMigrated("userTask1", "userTask1", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateTransitionInstanceJobProperties()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

            var jobBeforeMigration = rule.ManagementService.CreateJobQuery()
                .First();
            rule.ManagementService.SetJobPriority(jobBeforeMigration.Id, 42);

            // TODO: fix CAM-5692
            //    Date newDueDate = new DateTime().plusHours(10);
            //    rule.GetManagementService().SetJobDuedate(jobBeforeMigration.GetId(), newDueDate);
            rule.ManagementService.SetJobRetries(jobBeforeMigration.Id, 52);
            rule.ManagementService.SuspendJobById(jobBeforeMigration.Id);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var job = testHelper.SnapshotAfterMigration.Jobs[0];

            Assert.AreEqual(42, job.Priority);
            //    Assert.AreEqual(newDueDate, job.GetDuedate());
            Assert.AreEqual(52, job.Retries);
            Assert.True(job.Suspended);
        }

        [Test]
        public virtual void testMigrateAsyncBeforeStartEventTransitionInstanceCase1()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_START_EVENT_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_START_EVENT_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("startEvent", "startEvent")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertJobMigrated("startEvent", "startEvent", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            Assert.AreEqual(
                "Replace this non-API Assert with a proper test case that fails when the wrong atomic operation is used",
                "process-start", ((JobEntity) job).JobHandlerConfigurationRaw);
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncBeforeStartEventTransitionInstanceCase2()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_START_EVENT_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_START_EVENT_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("startEvent", "subProcessStart")
                    .Build();

            // when
            try
            {
                testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasTransitionInstanceFailures("startEvent",
                        "A transition instance that instantiates the process can only be migrated to a process-level flow node");
            }
        }

        [Test]
        public virtual void testMigrateAsyncBeforeStartEventTransitionInstanceCase3()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_START_EVENT_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_START_EVENT_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcessStart", "subProcessStart")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertJobMigrated("subProcessStart", "subProcessStart", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncBeforeStartEventTransitionInstanceCase4()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_START_EVENT_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_START_EVENT_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcessStart", "startEvent")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertJobMigrated("subProcessStart", "startEvent", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncBeforeTransitionInstanceAddParentScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_USER_TASK_PROCESS);

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
                    .BeginScope("subProcess")
                    .Transition("userTask")
                    .Done());

            testHelper.AssertJobMigrated("userTask", "userTask", AsyncContinuationJobHandler.TYPE);

            // and it is possible to successfully execute the migrated job
            var job = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncBeforeTransitionInstanceConcurrentAddParentScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_USER_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(migrationPlan.SourceProcessDefinitionId)
                .StartBeforeActivity("userTask")
                .StartBeforeActivity("userTask")
                .Execute();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Scope()
                    .Child("userTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("userTask")
                    .Concurrent()
                    .NoScope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
                    .Transition("userTask")
                    .Transition("userTask")
                    .Done());

            Assert.AreEqual(2, testHelper.SnapshotAfterMigration.Jobs.Count);

            // and it is possible to successfully execute the migrated job
            foreach (var job in testHelper.SnapshotAfterMigration.Jobs)
            {
                rule.ManagementService.ExecuteJob(job.Id);
                testHelper.CompleteTask("userTask");
            }

            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncBeforeTransitionInstanceWithIncident()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS)
                    .ChangeElementId("userTask", "newUserTask"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "newUserTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

            var job = rule.ManagementService.CreateJobQuery()
                .First();
            rule.ManagementService.SetJobRetries(job.Id, 0);

            var incidentBeforeMigration = rule.RuntimeService.CreateIncidentQuery()
                .First();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var incidentAfterMigration = rule.RuntimeService.CreateIncidentQuery()
                .First();

            Assert.NotNull(incidentAfterMigration);
            // and it is still the same incident
            Assert.AreEqual(incidentBeforeMigration.Id, incidentAfterMigration.Id);
            Assert.AreEqual(job.Id, incidentAfterMigration.Configuration);

            // and the activity and process definition references were updated
            Assert.AreEqual("newUserTask", incidentAfterMigration.ActivityId);
            Assert.AreEqual(targetProcessDefinition.Id, incidentAfterMigration.ProcessDefinitionId);

            // and it is possible to successfully execute the migrated job
            rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteTask("newUserTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }


        [Test]
        public virtual void testMigrateAsyncBeforeInnerMultiInstance()
        {
            // given
            IBpmnModelInstance model = ModifiableBpmnModelInstance.Modify(
                    MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS)
                .AsyncBeforeInnerMiActivity("userTask");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            var jobs = testHelper.SnapshotAfterMigration.Jobs;
            Assert.AreEqual(3, jobs.Count);

            testHelper.AssertJobMigrated(jobs[0], "userTask");
            testHelper.AssertJobMigrated(jobs[1], "userTask");
            testHelper.AssertJobMigrated(jobs[2], "userTask");

            // and it is possible to successfully execute the migrated jobs
            foreach (var job in jobs)
                rule.ManagementService.ExecuteJob(job.Id);

            // and complete the task and process instance
            testHelper.CompleteAnyTask("userTask");
            testHelper.CompleteAnyTask("userTask");
            testHelper.CompleteAnyTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateAsyncAfterInnerMultiInstance()
        {
            // given
            IBpmnModelInstance model = ModifiableBpmnModelInstance.Modify(
                    MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS)
                .AsyncAfterInnerMiActivity("userTask");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

            testHelper.CompleteAnyTask("userTask");
            testHelper.CompleteAnyTask("userTask");
            testHelper.CompleteAnyTask("userTask");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var jobs = testHelper.SnapshotAfterMigration.Jobs;
            Assert.AreEqual(3, jobs.Count);

            testHelper.AssertJobMigrated(jobs[0], "userTask");
            testHelper.AssertJobMigrated(jobs[1], "userTask");
            testHelper.AssertJobMigrated(jobs[2], "userTask");

            // and it is possible to successfully execute the migrated jobs
            foreach (var job in jobs)
                rule.ManagementService.ExecuteJob(job.Id);

            // and complete the process instance
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testCannotMigrateAsyncBeforeTransitionInstanceToNonAsyncActivity()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            // when
            try
            {
                testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasTransitionInstanceFailures("userTask", "Target activity is not asyncBefore");
            }
        }

        [Test]
        public virtual void testCannotMigrateAsyncAfterTransitionInstanceToNonAsyncActivity()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteTask("userTask1");

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
                    .HasTransitionInstanceFailures("userTask1", "Target activity is not asyncAfter");
            }
        }

        [Test]
        public virtual void testCannotMigrateUnmappedTransitionInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .Build();

            // when
            try
            {
                testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasTransitionInstanceFailures("userTask",
                        "There is no migration instruction for this instance's activity");
            }
        }

        [Test]
        public virtual void testCannotMigrateUnmappedTransitionInstanceAtNonLeafActivity()
        {
            // given
            IBpmnModelInstance model = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("subProcess")
                //.CamundaAsyncBefore(true)
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .Build();

            // when
            try
            {
                testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                MigratingProcessInstanceValidationReportAssert.That(e.ValidationReport)
                    .HasTransitionInstanceFailures("subProcess",
                        "There is no migration instruction for this instance's activity");
            }
        }

        [Test]
        public virtual void testCannotMigrateUnmappedTransitionInstanceWithIncident()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

            // the user task is not mapped in the migration plan, i.E. there is no instruction to migrate the job
            // and the incident
            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

            var job = rule.ManagementService.CreateJobQuery()
                .First();
            rule.ManagementService.SetJobRetries(job.Id, 0);

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
                    .HasTransitionInstanceFailures("userTask",
                        "There is no migration instruction for this instance's activity");
            }
        }

        [Test]
        public virtual void testMigrateAsyncBeforeTransitionInstanceToDifferentProcessKey()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS)
                    .ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertJobMigrated("userTask", "userTask", AsyncContinuationJobHandler.TYPE);
        }

        [Test]
        public virtual void testMigrateAsyncAfterCompensateEventSubProcessStartEvent()
        {
            // given
            IBpmnModelInstance model =
                    ModifiableBpmnModelInstance.Modify(EventSubProcessModels.COMPENSATE_EVENT_SUBPROCESS_PROCESS)
                //.FlowNodeBuilder("eventSubProcessStart")
                //.CamundaAsyncAfter()
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("eventSubProcess", "eventSubProcess")
                    .MapActivities("eventSubProcessStart", "eventSubProcessStart")
                    .Build();

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity("eventSubProcess")
                .Execute();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertJobMigrated("eventSubProcessStart", "eventSubProcessStart",
                AsyncContinuationJobHandler.TYPE);
        }

        /// <summary>
        ///     Does not apply since asyncAfter cannot be used with boundary events
        /// </summary>
        [Test]
        public virtual void testMigrateAsyncAfterBoundaryEventWithChangedEventScope()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                //.ActivityBuilder("userTask1")
                //.BoundaryEvent("boundary")
                //.Message("Message")
                //.CamundaAsyncAfter()
                //.UserTask("afterBoundaryTask")
                //.EndEvent()
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .SwapElementIds("userTask1", "userTask2");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("boundary", "boundary")
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertJobMigrated("boundary", "boundary", AsyncContinuationJobHandler.TYPE);
        }

        [Test]
        public virtual void testFailMigrateFailedJobIncident()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var model = ProcessModels.NewModel()
                .StartEvent()
                .ServiceTask("serviceTask")
               // .CamundaAsyncBefore()
                .CamundaClass(typeof(AlwaysFailingDelegate).FullName)
                .EndEvent()
                .Done();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(model)
                .ChangeElementId("serviceTask", "newServiceTask"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var ProcessInstanceId = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id)
                .Id;
            testHelper.ExecuteAvailableJobs();

            // when
            try
            {
                rule.RuntimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds(ProcessInstanceId)
                    .Execute();

                Assert.Fail("should Assert.Fail");
            }
            catch (MigratingProcessInstanceValidationException e)
            {
                // then
                Assert.True(e is MigratingProcessInstanceValidationException);
            }
        }
    }
}