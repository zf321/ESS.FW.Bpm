using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    [TestFixture]
    public class MigrationAddBoundaryEventsTest
    {
        public const string AFTER_BOUNDARY_TASK = "afterBoundary";
        public const string MESSAGE_NAME = "Message";
        public const string SIGNAL_NAME = "Signal";
        public const string TIMER_DATE = "2016-02-11T12:13:14Z";
        public const string ERROR_CODE = "Error";
        public const string ESCALATION_CODE = "Escalation";
        private readonly bool InstanceFieldsInitialized;

        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationAddBoundaryEventsTest()
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
        public virtual void testAddMessageBoundaryEventToUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToUserTaskAndCorrelateMessage()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the message and successfully complete the migrated instance
            testHelper.CorrelateMessage(MESSAGE_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToScopeUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToScopeUserTaskAndCorrelateMessage()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the message and successfully complete the migrated instance
            testHelper.CorrelateMessage(MESSAGE_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToConcurrentUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToConcurrentUserTaskAndCorrelateMessage()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the message and successfully complete the migrated instance
            testHelper.CorrelateMessage(MESSAGE_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToConcurrentScopeUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToConcurrentScopeUserTaskAndCorrelateMessage()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the message and successfully complete the migrated instance
            testHelper.CorrelateMessage(MESSAGE_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToSubProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToSubProcessAndCorrelateMessage()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the message and successfully complete the migrated instance
            testHelper.CorrelateMessage(MESSAGE_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToSubProcessWithScopeUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskSubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToSubProcessWithScopeUserTaskAndCorrelateMessage()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskSubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the message and successfully complete the migrated instance
            testHelper.CorrelateMessage(MESSAGE_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToParallelSubProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelSubprocessProcess)
                    //.ActivityBuilder("subProcess1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess1", "subProcess1")
                    .MapActivities("subProcess2", "subProcess2")
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMessageBoundaryEventToParallelSubProcessAndCorrelateMessage()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelSubprocessProcess)
                    //.ActivityBuilder("subProcess1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess1", "subProcess1")
                    .MapActivities("subProcess2", "subProcess2")
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the message and successfully complete the migrated instance
            testHelper.CorrelateMessage(MESSAGE_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToUserTaskAndSendSignal()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to send the signal and successfully complete the migrated instance
            testHelper.SendSignal(SIGNAL_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToScopeUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToScopeUserTaskAndSendSignal()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to send the signal and successfully complete the migrated instance
            testHelper.SendSignal(SIGNAL_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToConcurrentUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToConcurrentUserTaskAndSendSignal()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to send the signal and successfully complete the migrated instance
            testHelper.SendSignal(SIGNAL_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToConcurrentScopeUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToConcurrentScopeUserTaskAndSendSignal()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to send the signal and successfully complete the migrated instance
            testHelper.SendSignal(SIGNAL_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToSubProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToSubProcessAndCorrelateSignal()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the signal and successfully complete the migrated instance
            testHelper.SendSignal(SIGNAL_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToSubProcessWithScopeUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskSubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToSubProcessWithScopeUserTaskAndCorrelateSignal()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskSubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the signal and successfully complete the migrated instance
            testHelper.SendSignal(SIGNAL_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddSignalBoundaryEventToParallelSubProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelSubprocessProcess)
                    //.ActivityBuilder("subProcess1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess1", "subProcess1")
                    .MapActivities("subProcess2", "subProcess2")
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }
        [Test]
        public virtual void testAddSignalBoundaryEventToParallelSubProcessAndCorrelateSignal()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelSubprocessProcess)
                    //.ActivityBuilder("subProcess1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess1", "subProcess1")
                    .MapActivities("subProcess2", "subProcess2")
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the signal and successfully complete the migrated instance
            testHelper.SendSignal(SIGNAL_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobCreated("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToUserTaskAndSendTimerWithDate()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to send the timer and successfully complete the migrated instance
            testHelper.TriggerTimer();
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToScopeUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobCreated("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToScopeUserTaskAndSendTimerWithDate()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to send the timer and successfully complete the migrated instance
            testHelper.TriggerTimer();
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToConcurrentUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobCreated("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToConcurrentUserTaskAndSendTimerWithDate()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to send the timer and successfully complete the migrated instance
            testHelper.TriggerTimer();
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToConcurrentScopeUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobCreated("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToConcurrentScopeUserTaskAndSendTimerWithDate()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to send the timer and successfully complete the migrated instance
            testHelper.TriggerTimer();
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToSubProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobCreated("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToSubProcessAndCorrelateTimerWithDate()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the timer and successfully complete the migrated instance
            testHelper.TriggerTimer();
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToSubProcessWithScopeUserTask()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskSubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobCreated("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToSubProcessWithScopeUserTaskAndCorrelateTimerWithDate()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskSubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the timer and successfully complete the migrated instance
            testHelper.TriggerTimer();
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToParallelSubProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelSubprocessProcess)
                    //.ActivityBuilder("subProcess1")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess1", "subProcess1")
                    .MapActivities("subProcess2", "subProcess2")
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobCreated("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTimerBoundaryEventToParallelSubProcessAndCorrelateTimerWithDate()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelSubprocessProcess)
                    //.ActivityBuilder("subProcess1")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess1", "subProcess1")
                    .MapActivities("subProcess2", "subProcess2")
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to correlate the timer and successfully complete the migrated instance
            testHelper.TriggerTimer();
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddMultipleBoundaryEvents()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("timerBoundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.MoveToActivity("userTask")
                    //.BoundaryEvent("messageBoundary")
                    ////.Message(MESSAGE_NAME)
                    //.MoveToActivity("userTask")
                    //.BoundaryEvent("signalBoundary")
                    //.Signal(SIGNAL_NAME)
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("messageBoundary", MESSAGE_NAME);
            testHelper.AssertEventSubscriptionCreated("signalBoundary", SIGNAL_NAME);
            testHelper.AssertBoundaryTimerJobCreated("timerBoundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddErrorBoundaryEventToSubProcessAndThrowError()
        {
            // given
            //var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            //var targetProcessDefinition =
            //    testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
            //        .EndEventBuilder("subProcessEnd")
            //        .Error(ERROR_CODE)
            //        //.MoveToActivity("subProcess")
            //        //.BoundaryEvent()
            //        .Error(ERROR_CODE)
            //        .UserTask(AFTER_BOUNDARY_TASK)
            //        .EndEvent()
            //        .Done()); // let the end event of the subprocess throw an error

            //var migrationPlan =
            //    rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
            //        .MapActivities("subProcess", "subProcess")
            //        .MapActivities("userTask", "userTask")
            //        .Build();

            //// when
            //testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            //// then it is possible to successfully complete the migrated instance
            //testHelper.CompleteTask("userTask");
            //testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            //testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
            Assert.Fail();
        }

        [Test]
        public virtual void testAddEscalationBoundaryEventToSubProcessAndThrowEscalation()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    .EndEventBuilder("subProcessEnd")
                    .Escalation(ESCALATION_CODE)
                    //.MoveToActivity("subProcess")
                    //.BoundaryEvent()
                    .Escalation(ESCALATION_CODE)
                    .UserTask(AFTER_BOUNDARY_TASK)
                    .EndEvent()
                    .Done()); // catch escalation with boundary event -  let the end event of the subprocess escalate

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }
    }
}