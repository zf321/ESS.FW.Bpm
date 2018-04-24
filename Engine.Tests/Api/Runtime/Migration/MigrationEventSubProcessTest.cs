using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    [TestFixture]
    public class MigrationEventSubProcessTest
    {
        public const string SIGNAL_NAME = "Signal";
        protected internal const string EVENT_SUB_PROCESS_START_ID = "eventSubProcessStart";
        protected internal const string EVENT_SUB_PROCESS_TASK_ID = "eventSubProcessTask";
        protected internal const string USER_TASK_ID = "userTask";
        private readonly bool InstanceFieldsInitialized;

        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationEventSubProcessTest()
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
        //ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException exceptionRule = org.junit.Rules.ExpectedException.None();
        //public ExpectedException exceptionRule = ExpectedException.None();

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        [Test]
        public virtual void testMigrateActiveEventSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity(EVENT_SUB_PROCESS_TASK_ID)
                .Execute();

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventSubProcess", "eventSubProcess")
                    .MapActivities(EVENT_SUB_PROCESS_TASK_ID, EVENT_SUB_PROCESS_TASK_ID)
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(processInstance.Id)
                    .Child(EVENT_SUB_PROCESS_TASK_ID)
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("eventSubProcess"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("eventSubProcess",
                        testHelper.GetSingleActivityInstanceBeforeMigration("eventSubProcess")
                            .Id)
                    .Activity(EVENT_SUB_PROCESS_TASK_ID,
                        testHelper.GetSingleActivityInstanceBeforeMigration(EVENT_SUB_PROCESS_TASK_ID)
                            .Id)
                    .Done());

            testHelper.AssertEventSubscriptionRemoved(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME);
            testHelper.AssertEventSubscriptionCreated(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME);

            // and it is possible to complete the process instance
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateActiveEventSubProcessToEmbeddedSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity(EVENT_SUB_PROCESS_TASK_ID)
                .Execute();

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventSubProcess", "subProcess")
                    .MapActivities(EVENT_SUB_PROCESS_TASK_ID, USER_TASK_ID)
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(processInstance.Id)
                    .Child(USER_TASK_ID)
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("eventSubProcess"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess", testHelper.GetSingleActivityInstanceBeforeMigration("eventSubProcess")
                        .Id)
                    .Activity(USER_TASK_ID,
                        testHelper.GetSingleActivityInstanceBeforeMigration(EVENT_SUB_PROCESS_TASK_ID)
                            .Id)
                    .Done());

            testHelper.AssertEventSubscriptionRemoved(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME);
            Assert.AreEqual(0, testHelper.SnapshotAfterMigration.EventSubscriptions.Count);

            // and it is possible to complete the process instance
            testHelper.CompleteTask(USER_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateActiveEmbeddedSubProcessToEventSubProcess()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "eventSubProcess")
                    .MapActivities(USER_TASK_ID, EVENT_SUB_PROCESS_TASK_ID)
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(processInstance.Id)
                    .Child(EVENT_SUB_PROCESS_TASK_ID)
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("subProcess"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("eventSubProcess", testHelper.GetSingleActivityInstanceBeforeMigration("subProcess")
                        .Id)
                    .Activity(EVENT_SUB_PROCESS_TASK_ID,
                        testHelper.GetSingleActivityInstanceBeforeMigration(USER_TASK_ID)
                            .Id)
                    .Done());

            testHelper.AssertEventSubscriptionCreated(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME);

            // and it is possible to complete the process instance
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateActiveErrorEventSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.ERROR_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.ERROR_EVENT_SUBPROCESS_PROCESS);

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity(EVENT_SUB_PROCESS_TASK_ID)
                .Execute();

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventSubProcess", "eventSubProcess")
                    .MapActivities(EVENT_SUB_PROCESS_TASK_ID, EVENT_SUB_PROCESS_TASK_ID)
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then it is possible to complete the process instance
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateActiveCompensationEventSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.COMPENSATE_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.COMPENSATE_EVENT_SUBPROCESS_PROCESS);

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity(EVENT_SUB_PROCESS_TASK_ID)
                .Execute();

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventSubProcess", "eventSubProcess")
                    .MapActivities(EVENT_SUB_PROCESS_TASK_ID, EVENT_SUB_PROCESS_TASK_ID)
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then it is possible to complete the process instance
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateActiveEscalationEventSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.ESCALATION_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.ESCALATION_EVENT_SUBPROCESS_PROCESS);

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity(EVENT_SUB_PROCESS_TASK_ID)
                .Execute();

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventSubProcess", "eventSubProcess")
                    .MapActivities(EVENT_SUB_PROCESS_TASK_ID, EVENT_SUB_PROCESS_TASK_ID)
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then it is possible to complete the process instance
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateTaskAddEventSubProcess()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, EVENT_SUB_PROCESS_TASK_ID)
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(EVENT_SUB_PROCESS_TASK_ID)
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("eventSubProcess")
                    .Activity(EVENT_SUB_PROCESS_TASK_ID,
                        testHelper.GetSingleActivityInstanceBeforeMigration(USER_TASK_ID)
                            .Id)
                    .Done());

            testHelper.AssertEventSubscriptionCreated(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME);

            // and it is possible to complete the process instance
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubprocessMessageKeepTrigger()
        {
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID)
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID,
                EventSubProcessModels.MESSAGE_NAME);

            // and it is possible to trigger the event subprocess
            rule.RuntimeService.CorrelateMessage(EventSubProcessModels.MESSAGE_NAME);
            Assert.AreEqual(1, rule.TaskService.CreateTaskQuery()
                .Count());

            // and complete the process instance
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubprocessTimerKeepTrigger()
        {
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID)
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertJobMigrated(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID,
                TimerStartEventSubprocessJobHandler.TYPE);

            // and it is possible to trigger the event subprocess
            var timerJob = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(timerJob.Id);
            Assert.AreEqual(1, rule.TaskService.CreateTaskQuery()
                .Count());

            // and complete the process instance
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubprocessSignalKeepTrigger()
        {
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID)
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID,
                EventSubProcessModels.SIGNAL_NAME);

            // and it is possible to trigger the event subprocess
            rule.RuntimeService.SignalEventReceived(EventSubProcessModels.SIGNAL_NAME);
            Assert.AreEqual(1, rule.TaskService.CreateTaskQuery()
                .Count());

            // and complete the process instance
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        //[ExpectedException(typeof(MigrationPlanValidationException))]
        public virtual void testMigrateConditionalBoundaryEventKeepTrigger()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.CONDITIONAL_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.CONDITIONAL_EVENT_SUBPROCESS_PROCESS);

            // expected migration validation exception
            //exceptionRule.Expect(typeof(MigrationPlanValidationException));
            //exceptionRule.ExpectMessage(MIGRATION_CONDITIONAL_VALIDATION_ERROR_MSG);

            // when conditional event sub process is migrated without update event trigger
            rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                .MapActivities(USER_TASK_ID, USER_TASK_ID)
                .MapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID)
                .Build();
        }


        [Test]
        public virtual void testMigrateEventSubprocessChangeStartEventType()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);

            try
            {
                // when
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID)
                    .Build();
                Assert.Fail("exception expected");
            }
            catch (MigrationPlanValidationException e)
            {
                // then
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures(EVENT_SUB_PROCESS_START_ID,
                        "Events are not of the same type (signalStartEvent != startTimerEvent)");
            }
        }

        [Test]
        public virtual void testMigrateEventSubprocessTimerIncident()
        {
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID)
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var timerTriggerJob = rule.ManagementService.CreateJobQuery()
                .First();
            // create an incident
            rule.ManagementService.SetJobRetries(timerTriggerJob.Id, 0);
            var incidentBeforeMigration = rule.RuntimeService.CreateIncidentQuery()
                .First();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            var incidentAfterMigration = rule.RuntimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(incidentAfterMigration);

            Assert.AreEqual(incidentBeforeMigration.Id, incidentAfterMigration.Id);
            Assert.AreEqual(timerTriggerJob.Id, incidentAfterMigration.Configuration);

            Assert.AreEqual(EVENT_SUB_PROCESS_START_ID, incidentAfterMigration.ActivityId);
            Assert.AreEqual(targetProcessDefinition.Id, incidentAfterMigration.ProcessDefinitionId);

            // and it is possible to complete the process
            rule.ManagementService.ExecuteJob(timerTriggerJob.Id);
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateNonInterruptingEventSubprocessMessageTrigger()
        {
            var nonInterruptingModel =
                ModifiableBpmnModelInstance.Modify(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS)
                    .StartEventBuilder(EVENT_SUB_PROCESS_START_ID)
                    .Interrupting(false)
                    .Done();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(nonInterruptingModel);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(nonInterruptingModel);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID)
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID,
                EventSubProcessModels.MESSAGE_NAME);

            // and it is possible to trigger the event subprocess
            rule.RuntimeService.CorrelateMessage(EventSubProcessModels.MESSAGE_NAME);
            Assert.AreEqual(2, rule.TaskService.CreateTaskQuery()
                .Count());

            // and complete the process instance
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.CompleteTask(USER_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testUpdateEventMessage()
        {
            // given
            var sourceProcess = EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(
                    EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS)
                .RenameMessage(EventSubProcessModels.MESSAGE_NAME, "new" + EventSubProcessModels.MESSAGE_NAME);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.MESSAGE_NAME,
                EVENT_SUB_PROCESS_START_ID, "new" + EventSubProcessModels.MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            rule.RuntimeService.CorrelateMessage("new" + EventSubProcessModels.MESSAGE_NAME);
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testUpdateEventSignal()
        {
            // given
            var sourceProcess = EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(
                    EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS)
                .RenameSignal(EventSubProcessModels.SIGNAL_NAME, "new" + EventSubProcessModels.SIGNAL_NAME);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.SIGNAL_NAME,
                EVENT_SUB_PROCESS_START_ID, "new" + EventSubProcessModels.SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            rule.RuntimeService.SignalEventReceived("new" + EventSubProcessModels.SIGNAL_NAME);
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testUpdateEventTimer()
        {
            // given
            ClockTestUtil.SetClockToDateWithoutMilliseconds();

            var sourceProcess = EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS;
            var targetProcess = ModifiableBpmnModelInstance.Modify(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS)
                .RemoveChildren(EVENT_SUB_PROCESS_START_ID)
                .StartEventBuilder(EVENT_SUB_PROCESS_START_ID)
                .TimerWithDuration("PT50M")
                .Done();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            var newDueDate = new DateTime(ClockUtil.CurrentTime.Ticks).AddMinutes(50);
            testHelper.AssertJobMigrated(testHelper.SnapshotBeforeMigration.Jobs[0], EVENT_SUB_PROCESS_START_ID,
                newDueDate);

            // and it is possible to successfully complete the migrated instance
            var jobAfterMigration = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(jobAfterMigration.Id);

            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testUpdateEventMessageWithExpression()
        {
            // given
            var newMessageNameWithExpression = "new" + EventSubProcessModels.MESSAGE_NAME + "-${var}";
            var sourceProcess = EventSubProcessModels.MESSAGE_INTERMEDIATE_EVENT_SUBPROCESS_PROCESS;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(
                    EventSubProcessModels.MESSAGE_INTERMEDIATE_EVENT_SUBPROCESS_PROCESS)
                .RenameMessage(EventSubProcessModels.MESSAGE_NAME, newMessageNameWithExpression);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities("eventSubProcess", "eventSubProcess")
                    .MapActivities("catchMessage", "catchMessage")
                    .UpdateEventTrigger()
                    .Build();
            var variables = new Dictionary<string, object>();
            variables["var"] = "foo";

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan, variables);

            // then
            var resolvedMessageName = "new" + EventSubProcessModels.MESSAGE_NAME + "-foo";
            testHelper.AssertEventSubscriptionMigrated("catchMessage", EventSubProcessModels.MESSAGE_NAME,
                "catchMessage", resolvedMessageName);

            // and it is possible to successfully complete the migrated instance
            rule.RuntimeService.CorrelateMessage(resolvedMessageName);
            testHelper.CompleteTask(USER_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }
        [Test]
        public virtual void testUpdateEventSignalWithExpression()
        {
            // given
            var newSignalNameWithExpression = "new" + EventSubProcessModels.MESSAGE_NAME + "-${var}";
            var sourceProcess = EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(
                    EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS)
                .RenameSignal(EventSubProcessModels.SIGNAL_NAME, newSignalNameWithExpression);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID)
                    .UpdateEventTrigger()
                    .Build();

            var variables = new Dictionary<string, object>();
            variables["var"] = "foo";

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan, variables);

            // then
            var resolvedsignalName = "new" + EventSubProcessModels.MESSAGE_NAME + "-foo";
            testHelper.AssertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EventSubProcessModels.SIGNAL_NAME,
                EVENT_SUB_PROCESS_START_ID, resolvedsignalName);

            // and it is possible to successfully complete the migrated instance
            rule.RuntimeService.SignalEventReceived(resolvedsignalName);
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testUpdateConditionalEventExpression()
        {
            // given
            var sourceProcess = EventSubProcessModels.FALSE_CONDITIONAL_EVENT_SUBPROCESS_PROCESS;
            IBpmnModelInstance targetProcess =
                ModifiableBpmnModelInstance.Modify(EventSubProcessModels.CONDITIONAL_EVENT_SUBPROCESS_PROCESS);

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);


            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when process is migrated without update event trigger
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then condition is migrated and has new condition expr
            testHelper.AssertEventSubscriptionMigrated(EVENT_SUB_PROCESS_START_ID, EVENT_SUB_PROCESS_START_ID, null);

            // and it is possible to successfully complete the migrated instance
            testHelper.AnyVariable = testHelper.SnapshotAfterMigration.ProcessInstanceId;
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }
    }
}