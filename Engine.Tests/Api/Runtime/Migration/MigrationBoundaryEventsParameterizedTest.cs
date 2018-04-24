using System.Collections.Generic;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Api.Runtime.Migration.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class MigrationBoundaryEventsParameterizedTest
    [TestFixture]
    public class MigrationBoundaryEventsParameterizedTest
    {
        public const string AFTER_BOUNDARY_TASK = "afterBoundary";
        public const string MESSAGE_NAME = "Message";
        public const string SIGNAL_NAME = "Signal";
        public const string TIMER_DATE = "2016-02-11T12:13:14Z";
        public const string NEW_TIMER_DATE = "2018-02-11T12:13:14Z";
        protected internal const string BOUNDARY_ID = "boundary";
        protected internal const string MIGRATE_MESSAGE_BOUNDARY_EVENT = "MigrateMessageBoundaryEvent";
        protected internal const string MIGRATE_SIGNAL_BOUNDARY_EVENT = "MigrateSignalBoundaryEvent";
        protected internal const string MIGRATE_TIMER_BOUNDARY_EVENT = "MigrateTimerBoundaryEvent";
        protected internal const string MIGRATE_CONDITIONAL_BOUNDARY_EVENT = "MigrateConditionalBoundaryEvent";
        protected internal const string USER_TASK_ID = "userTask";
        protected internal const string NEW_BOUNDARY_ID = "newBoundary";
        public const string USER_TASK_1_ID = "userTask1";
        public const string USER_TASK_2_ID = "userTask2";
        public const string SUB_PROCESS_ID = "subProcess";

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter public api.Runtime.Migration.util.BpmnEventFactory eventFactory;
        public BpmnEventFactory eventFactory;
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationBoundaryEventsParameterizedTest()
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
//ORIGINAL LINE: @Parameters public static java.util.Collection<Object[]> data()
        public static ICollection<object[]> data()
        {
            return new[]
            {
                new object[] {new TimerEventFactory()},
                new object[] {new MessageEventFactory()},
                new object[] {new SignalEventFactory()},
                new object[] {new ConditionalEventFactory()}
            };
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        // tests ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void testMigrateBoundaryEventOnUserTask()
        {
            // given
            var sourceProcess = ProcessModels.OneTaskProcess.Clone();
            var eventTrigger = eventFactory.AddBoundaryEvent(rule.ProcessEngine, sourceProcess,
                USER_TASK_ID, BOUNDARY_ID);
            ModifiableBpmnModelInstance.Wrap(sourceProcess)
                //.FlowNodeBuilder(BOUNDARY_ID)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            eventTrigger.AssertEventTriggerMigrated(testHelper, NEW_BOUNDARY_ID);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask(USER_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateBoundaryEventOnUserTaskAndTriggerEvent()
        {
            // given
            var sourceProcess = ProcessModels.OneTaskProcess.Clone();
            var eventTrigger = eventFactory.AddBoundaryEvent(rule.ProcessEngine, sourceProcess,
                USER_TASK_ID, BOUNDARY_ID);
            ModifiableBpmnModelInstance.Wrap(sourceProcess)
                //.FlowNodeBuilder(BOUNDARY_ID)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to trigger boundary event and successfully complete the migrated instance
            eventTrigger.InContextOf(NEW_BOUNDARY_ID)
                .Trigger(processInstance.Id);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateBoundaryEventOnConcurrentUserTask()
        {
            // given
            var sourceProcess = ProcessModels.ParallelGatewayProcess.Clone();
            var eventTrigger = eventFactory.AddBoundaryEvent(rule.ProcessEngine, sourceProcess,
                USER_TASK_1_ID, BOUNDARY_ID);
            ModifiableBpmnModelInstance.Wrap(sourceProcess)
                //.FlowNodeBuilder(BOUNDARY_ID)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId("boundary", "newBoundary");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);


            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_1_ID, USER_TASK_1_ID)
                    .MapActivities(USER_TASK_2_ID, USER_TASK_2_ID)
                    .MapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            eventTrigger.AssertEventTriggerMigrated(testHelper, NEW_BOUNDARY_ID);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask(USER_TASK_1_ID);
            testHelper.CompleteTask(USER_TASK_2_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateBoundaryEventOnConcurrentUserTaskAndTriggerEvent()
        {
            // given
            var sourceProcess = ProcessModels.ParallelGatewayProcess.Clone();
            var eventTrigger = eventFactory.AddBoundaryEvent(rule.ProcessEngine, sourceProcess,
                USER_TASK_1_ID, BOUNDARY_ID);
            ModifiableBpmnModelInstance.Wrap(sourceProcess)
                //.FlowNodeBuilder(BOUNDARY_ID)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId("boundary", "newBoundary");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);


            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_1_ID, USER_TASK_1_ID)
                    .MapActivities(USER_TASK_2_ID, USER_TASK_2_ID)
                    .MapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to trigger the event and successfully complete the migrated instance
            eventTrigger.InContextOf(NEW_BOUNDARY_ID)
                .Trigger(processInstance.Id);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.CompleteTask(USER_TASK_2_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateBoundaryEventOnConcurrentScopeUserTask()
        {
            // given
            var sourceProcess = ProcessModels.ScopeTaskProcess.Clone();
            var eventTrigger = eventFactory.AddBoundaryEvent(rule.ProcessEngine, sourceProcess,
                USER_TASK_1_ID, BOUNDARY_ID);
            ModifiableBpmnModelInstance.Wrap(sourceProcess)
                //.FlowNodeBuilder(BOUNDARY_ID)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId("boundary", "newBoundary");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);


            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_1_ID, USER_TASK_1_ID)
                    .MapActivities(USER_TASK_2_ID, USER_TASK_2_ID)
                    .MapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            eventTrigger.AssertEventTriggerMigrated(testHelper, NEW_BOUNDARY_ID);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask(USER_TASK_1_ID);
            testHelper.CompleteTask(USER_TASK_2_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateBoundaryEventOnConcurrentScopeUserTaskAndTriggerEvent()
        {
            // given
            var sourceProcess = ProcessModels.ScopeTaskProcess.Clone();
            var eventTrigger = eventFactory.AddBoundaryEvent(rule.ProcessEngine, sourceProcess,
                USER_TASK_1_ID, BOUNDARY_ID);
            ModifiableBpmnModelInstance.Wrap(sourceProcess)
                //.FlowNodeBuilder(BOUNDARY_ID)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId("boundary", "newBoundary");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);


            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_1_ID, USER_TASK_1_ID)
                    .MapActivities(USER_TASK_2_ID, USER_TASK_2_ID)
                    .MapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to trigger the event and successfully complete the migrated instance
            eventTrigger.InContextOf(NEW_BOUNDARY_ID)
                .Trigger(processInstance.Id);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.CompleteTask(USER_TASK_2_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateBoundaryEventToSubProcess()
        {
            // given
            var sourceProcess = ProcessModels.SubprocessProcess.Clone();
            var eventTrigger = eventFactory.AddBoundaryEvent(rule.ProcessEngine, sourceProcess,
                SUB_PROCESS_ID, BOUNDARY_ID);
            ModifiableBpmnModelInstance.Wrap(sourceProcess)
                //.FlowNodeBuilder(BOUNDARY_ID)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(SUB_PROCESS_ID, SUB_PROCESS_ID)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            eventTrigger.AssertEventTriggerMigrated(testHelper, NEW_BOUNDARY_ID);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask(USER_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateBoundaryEventToSubProcessAndTriggerEvent()
        {
            // given
            var sourceProcess = ProcessModels.SubprocessProcess.Clone();
            var eventTrigger = eventFactory.AddBoundaryEvent(rule.ProcessEngine, sourceProcess,
                SUB_PROCESS_ID, BOUNDARY_ID);
            ModifiableBpmnModelInstance.Wrap(sourceProcess)
                //.FlowNodeBuilder(BOUNDARY_ID)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(SUB_PROCESS_ID, SUB_PROCESS_ID)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to trigger the event and successfully complete the migrated instance
            eventTrigger.InContextOf(NEW_BOUNDARY_ID)
                .Trigger(processInstance.Id);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateBoundaryEventToSubProcessWithScopeUserTask()
        {
            // given
            var sourceProcess = ProcessModels.ScopeTaskSubprocessProcess.Clone();
            var eventTrigger = eventFactory.AddBoundaryEvent(rule.ProcessEngine, sourceProcess,
                SUB_PROCESS_ID, BOUNDARY_ID);
            ModifiableBpmnModelInstance.Wrap(sourceProcess)
                //.FlowNodeBuilder(BOUNDARY_ID)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(SUB_PROCESS_ID, SUB_PROCESS_ID)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            eventTrigger.AssertEventTriggerMigrated(testHelper, NEW_BOUNDARY_ID);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask(USER_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateBoundaryEventToSubProcessWithScopeUserTaskAndTriggerEvent()
        {
            // given
            var sourceProcess = ProcessModels.ScopeTaskSubprocessProcess.Clone();
            var eventTrigger = eventFactory.AddBoundaryEvent(rule.ProcessEngine, sourceProcess,
                SUB_PROCESS_ID, BOUNDARY_ID);
            ModifiableBpmnModelInstance.Wrap(sourceProcess)
                //.FlowNodeBuilder(BOUNDARY_ID)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(SUB_PROCESS_ID, SUB_PROCESS_ID)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to trigger the event and successfully complete the migrated instance
            eventTrigger.InContextOf(NEW_BOUNDARY_ID)
                .Trigger(processInstance.Id);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateBoundaryEventToParallelSubProcess()
        {
            // given
            var sourceProcess = ProcessModels.ParallelSubprocessProcess.Clone();
            var eventTrigger = eventFactory.AddBoundaryEvent(rule.ProcessEngine, sourceProcess,
                "subProcess1", BOUNDARY_ID);
            ModifiableBpmnModelInstance.Wrap(sourceProcess)
                //.FlowNodeBuilder(BOUNDARY_ID)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess1", "subProcess1")
                    .MapActivities(USER_TASK_1_ID, USER_TASK_1_ID)
                    .MapActivities("subProcess2", "subProcess2")
                    .MapActivities(USER_TASK_2_ID, USER_TASK_2_ID)
                    .MapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            eventTrigger.AssertEventTriggerMigrated(testHelper, NEW_BOUNDARY_ID);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask(USER_TASK_1_ID);
            testHelper.CompleteTask(USER_TASK_2_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateBoundaryEventToParallelSubProcessAndTriggerEvent()
        {
            // given
            var sourceProcess = ProcessModels.ParallelSubprocessProcess.Clone();
            var eventTrigger = eventFactory.AddBoundaryEvent(rule.ProcessEngine, sourceProcess,
                "subProcess1", BOUNDARY_ID);
            ModifiableBpmnModelInstance.Wrap(sourceProcess)
                //.FlowNodeBuilder(BOUNDARY_ID)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId(BOUNDARY_ID, NEW_BOUNDARY_ID);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess1", "subProcess1")
                    .MapActivities(USER_TASK_1_ID, USER_TASK_1_ID)
                    .MapActivities("subProcess2", "subProcess2")
                    .MapActivities(USER_TASK_2_ID, USER_TASK_2_ID)
                    .MapActivities(BOUNDARY_ID, NEW_BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to trigger the event and successfully complete the migrated instance
            eventTrigger.InContextOf(NEW_BOUNDARY_ID)
                .Trigger(processInstance.Id);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.CompleteTask(USER_TASK_2_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }
    }
}