using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    [TestFixture]
    public class MigrationRemoveBoundaryEventsTest
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

        public MigrationRemoveBoundaryEventsTest()
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
        public virtual void testRemoveMessageBoundaryEventFromUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveMessageBoundaryEventFromScopeUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveMessageBoundaryEventFromConcurrentUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveMessageBoundaryEventFromConcurrentScopeUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    /*.ActivityBuilder("userTask1").BoundaryEvent("boundary")//.Message(MESSAGE_NAME).UserTask(AFTER_BOUNDARY_TASK).EndEvent().Done()*/);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveMessageBoundaryEventFromSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveMessageBoundaryEventFromSubProcessWithScopeUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskSubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskSubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveMessageBoundaryEventFromParallelSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelSubprocessProcess)
                    //.ActivityBuilder("subProcess1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);

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
            testHelper.AssertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveMessageBoundaryEventFromUserTaskInSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveSignalBoundaryEventFromUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveSignalBoundaryEventFromScopeUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveSignalBoundaryEventFromConcurrentUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveSignalBoundaryEventFromConcurrentScopeUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveSignalBoundaryEventFromSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveSignalBoundaryEventFromSubProcessWithScopeUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskSubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskSubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveSignalBoundaryEventFromParallelSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelSubprocessProcess)
                    //.ActivityBuilder("subProcess1")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);

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
            testHelper.AssertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveSignalBoundaryEventFromUserTaskInSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("boundary", SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveTimerBoundaryEventFromUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobRemoved("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }
        [Test]
        public virtual void testRemoveTimerBoundaryEventFromScopeUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobRemoved("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveTimerBoundaryEventFromConcurrentUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobRemoved("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveTimerBoundaryEventFromConcurrentScopeUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskProcess)
                    //.ActivityBuilder("userTask1")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobRemoved("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveTimerBoundaryEventFromSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobRemoved("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveTimerBoundaryEventFromSubProcessWithScopeUserTask()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ScopeTaskSubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskSubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobRemoved("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveTimerBoundaryEventFromParallelSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelSubprocessProcess)
                    //.ActivityBuilder("subProcess1")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);

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
            testHelper.AssertBoundaryTimerJobRemoved("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask1");
            testHelper.CompleteTask("userTask2");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveTimerBoundaryEventFromUserTaskInSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    //.TimerWithDate(TIMER_DATE)
                    //.UserTask(AFTER_BOUNDARY_TASK)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertBoundaryTimerJobRemoved("boundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveMultipleBoundaryEvents()
        {
            // given
            var sourceProcessDefinition =
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
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("messageBoundary", MESSAGE_NAME);
            testHelper.AssertEventSubscriptionRemoved("signalBoundary", SIGNAL_NAME);
            testHelper.AssertBoundaryTimerJobRemoved("timerBoundary");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveErrorBoundaryEventFromSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent()
                    //.Error(ERROR_CODE)
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveEscalationBoundaryEventFromSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.BoundaryEvent()
                    //.Escalation(ESCALATION_CODE)
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to successfully complete the migrated instance
            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveIncidentForJob()
        {
            // given
            var sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .UserTaskBuilder("userTask")
                .BoundaryEvent("boundary")
                .TimerWithDate(TIMER_DATE)
                .ServiceTask("failingTask")
                .CamundaClass("api.Runtime.FailingDelegate")
                .EndEvent()
                .Done();
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId("userTask", "newUserTask")
                .ChangeElementId("boundary", "newBoundary");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            // a timer job exists
            var jobBeforeMigration = rule.ManagementService.CreateJobQuery()
                .First();
            Assert.NotNull(jobBeforeMigration);

            // if the timer job is triggered the failing delegate fails and an incident is created
            executeJob(jobBeforeMigration);
            var incidentBeforeMigration = rule.RuntimeService.CreateIncidentQuery()
                .First();
            Assert.AreEqual("boundary", incidentBeforeMigration.ActivityId);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "newUserTask")
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the incident was removed
            var jobAfterMigration = rule.ManagementService.CreateJobQuery(c=>c.Id == jobBeforeMigration.Id)
                .First();
            Assert.IsNull(jobAfterMigration);

            Assert.AreEqual(0, rule.RuntimeService.CreateIncidentQuery()
                .Count());
        }

        protected internal virtual void executeJob(IJob job)
        {
            var managementService = rule.ManagementService;

            while (job != null && job.Retries > 0)
            {
                try
                {
                    managementService.ExecuteJob(job.Id);
                }
                catch (System.Exception)
                {
                    // ignore
                }

                job = managementService.CreateJobQuery(c=>c.Id==job.Id)
                    .First();
            }
        }
    }
}