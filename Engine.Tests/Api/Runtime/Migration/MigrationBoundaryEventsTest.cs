using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    [TestFixture]
    public class MigrationBoundaryEventsTest
    {
        public const string AFTER_BOUNDARY_TASK = "afterBoundary";
        public const string MESSAGE_NAME = "Message";
        public const string SIGNAL_NAME = "Signal";
        public const string TIMER_DATE = "2016-02-11T12:13:14Z";
        protected internal const string FALSE_CONDITION = "${false}";
        protected internal const string VAR_CONDITION = "${any=='any'}";
        protected internal const string BOUNDARY_ID = "boundary";
        protected internal const string USER_TASK_ID = "userTask";
        private readonly bool InstanceFieldsInitialized;

        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationBoundaryEventsTest()
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
        public virtual void testMigrateMultipleBoundaryEvents()
        {
            // given
            IBpmnModelInstance testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("subProcess")
                //.BoundaryEvent("timerBoundary1")
                //.TimerWithDate(TIMER_DATE)
                //.MoveToActivity("subProcess")
                //.BoundaryEvent("messageBoundary1")
                ////.Message(MESSAGE_NAME)
                //.MoveToActivity("subProcess")
                //.BoundaryEvent("signalBoundary1")
                //.Signal(SIGNAL_NAME)
                //.MoveToActivity("subProcess")
                //.BoundaryEvent("conditionalBoundary1")
                //.Condition(VAR_CONDITION)
                //.MoveToActivity(USER_TASK_ID)
                //.BoundaryEvent("timerBoundary2")
                //.TimerWithDate(TIMER_DATE)
                //.MoveToActivity(USER_TASK_ID)
                //.BoundaryEvent("messageBoundary2")
                ////.Message(MESSAGE_NAME)
                //.MoveToActivity(USER_TASK_ID)
                //.BoundaryEvent("signalBoundary2")
                //.Signal(SIGNAL_NAME)
                //.MoveToActivity(USER_TASK_ID)
                //.BoundaryEvent("conditionalBoundary2")
                //.Condition(VAR_CONDITION)
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("timerBoundary1", "timerBoundary1")
                    .MapActivities("signalBoundary1", "signalBoundary1")
                    .MapActivities("conditionalBoundary1", "conditionalBoundary1")
                    .UpdateEventTrigger()
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities("messageBoundary2", "messageBoundary2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("messageBoundary1", MESSAGE_NAME);
            testHelper.AssertEventSubscriptionRemoved("signalBoundary2", SIGNAL_NAME);
            testHelper.AssertEventSubscriptionRemoved("conditionalBoundary2", null);
            testHelper.AssertEventSubscriptionMigrated("signalBoundary1", "signalBoundary1", SIGNAL_NAME);
            testHelper.AssertEventSubscriptionMigrated("messageBoundary2", "messageBoundary2", MESSAGE_NAME);
            testHelper.AssertEventSubscriptionMigrated("conditionalBoundary1", "conditionalBoundary1", null);
            testHelper.AssertEventSubscriptionCreated("messageBoundary1", MESSAGE_NAME);
            testHelper.AssertEventSubscriptionCreated("signalBoundary2", SIGNAL_NAME);
            testHelper.AssertEventSubscriptionCreated("conditionalBoundary2", null);
            testHelper.AssertBoundaryTimerJobRemoved("timerBoundary2");
            testHelper.AssertBoundaryTimerJobMigrated("timerBoundary1", "timerBoundary1");
            testHelper.AssertBoundaryTimerJobCreated("timerBoundary2");

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask(USER_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }
        [Test]
        public virtual void testMigrateBoundaryEventAndEventSubProcess()
        {
            var testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                .AddSubProcessTo("subProcess")
                .TriggerByEvent()
                ////.EmbeddedSubProcess()
                //.StartEvent("eventStart")
                ////.Message(MESSAGE_NAME)
                .EndEvent()
                .SubProcessDone()
                //.MoveToActivity(USER_TASK_ID)
                .BoundaryEvent(BOUNDARY_ID)
                .Signal(SIGNAL_NAME)
                .Done();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(BOUNDARY_ID, BOUNDARY_ID)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("eventStart", MESSAGE_NAME);
            testHelper.AssertEventSubscriptionMigrated(BOUNDARY_ID, BOUNDARY_ID, SIGNAL_NAME);
            testHelper.AssertEventSubscriptionCreated("eventStart", MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask(USER_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateIncidentForJob()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .UserTaskBuilder(USER_TASK_ID)
                .BoundaryEvent(BOUNDARY_ID)
                .TimerWithDate(TIMER_DATE)
                .ServiceTask("failingTask")
                .CamundaClass(typeof(FailingDelegate).FullName)
                .EndEvent()
                .Done();
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId(USER_TASK_ID, "newUserTask")
                .ChangeElementId(BOUNDARY_ID, "newBoundary");

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

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, "newUserTask")
                    .MapActivities(BOUNDARY_ID, "newBoundary")
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the job and incident still exists
            var jobAfterMigration = rule.ManagementService.CreateJobQuery(c=>c.Id == jobBeforeMigration.Id)
                .First();
            Assert.NotNull(jobAfterMigration);
            var incidentAfterMigration = rule.RuntimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(incidentAfterMigration);

            // and it is still the same incident
            Assert.AreEqual(incidentBeforeMigration.Id, incidentAfterMigration.Id);
            Assert.AreEqual(jobAfterMigration.Id, incidentAfterMigration.Configuration);

            // and the activity, process definition and job definition references were updated
            Assert.AreEqual("newBoundary", incidentAfterMigration.ActivityId);
            Assert.AreEqual(targetProcessDefinition.Id, incidentAfterMigration.ProcessDefinitionId);
            Assert.AreEqual(jobAfterMigration.JobDefinitionId, incidentAfterMigration.JobDefinitionId);
        }

        [Test]
        public virtual void testUpdateEventMessage()
        {
            // given
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                ////.Message(MESSAGE_NAME)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.Message("new" + MESSAGE_NAME)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated(BOUNDARY_ID, MESSAGE_NAME, BOUNDARY_ID, "new" + MESSAGE_NAME);

            // and it is possible to successfully complete the migrated instance
            rule.RuntimeService.CorrelateMessage("new" + MESSAGE_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testUpdateEventSignal()
        {
            // given
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.Signal(SIGNAL_NAME)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.Signal("new" + SIGNAL_NAME)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated(BOUNDARY_ID, SIGNAL_NAME, BOUNDARY_ID, "new" + SIGNAL_NAME);

            // and it is possible to successfully complete the migrated instance
            rule.RuntimeService.SignalEventReceived("new" + SIGNAL_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testUpdateEventTimer()
        {
            // given
            ClockTestUtil.SetClockToDateWithoutMilliseconds();

            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.TimerWithDate(TIMER_DATE)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.TimerWithDuration("PT50M")
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            var newDueDate = new DateTime(ClockUtil.CurrentTime.Ticks).AddMinutes(50);
            testHelper.AssertJobMigrated(testHelper.SnapshotBeforeMigration.Jobs[0], BOUNDARY_ID, newDueDate);

            // and it is possible to successfully complete the migrated instance
            var jobAfterMigration = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(jobAfterMigration.Id);

            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
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

        [Test]
        public virtual void testUpdateEventSignalNameWithExpression()
        {
            // given
            var signalNameWithExpression = "new" + SIGNAL_NAME + "-${var}";
            var sourceProcess = ProcessModels.OneTaskProcess;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.Signal(signalNameWithExpression)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .Build();

            var variables = new Dictionary<string, object>();
            variables["var"] = "foo";

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan, variables);

            // the signal event subscription's event name has changed
            var resolvedSignalName = "new" + SIGNAL_NAME + "-foo";
            testHelper.AssertEventSubscriptionCreated(BOUNDARY_ID, resolvedSignalName);

            // and it is possible to successfully complete the migrated instance
            rule.RuntimeService.SignalEventReceived(resolvedSignalName);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testUpdateEventMessageNameWithExpression()
        {
            // given
            var messageNameWithExpression = "new" + MESSAGE_NAME + "-${var}";
            var sourceProcess = ProcessModels.OneTaskProcess;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.Message(messageNameWithExpression)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .Build();

            var variables = new Dictionary<string, object>();
            variables["var"] = "foo";

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan, variables);

            // the message event subscription's event name has changed
            var resolvedMessageName = "new" + MESSAGE_NAME + "-foo";
            testHelper.AssertEventSubscriptionCreated(BOUNDARY_ID, resolvedMessageName);

            // and it is possible to successfully complete the migrated instance
            rule.RuntimeService.CorrelateMessage(resolvedMessageName);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }


        [Test]
        public virtual void testUpdateConditionalEventExpression()
        {
            // given
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.Condition(FALSE_CONDITION)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.Condition(VAR_CONDITION)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when process is migrated without update event trigger
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then condition is migrated and has new condition expr
            testHelper.AssertEventSubscriptionMigrated(BOUNDARY_ID, BOUNDARY_ID, null);

            // and it is possible to successfully complete the migrated instance
            testHelper.AnyVariable = testHelper.SnapshotAfterMigration.ProcessInstanceId;
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateSignalBoundaryEventKeepTrigger()
        {
            // given
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.Signal(SIGNAL_NAME)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.Signal("new" + SIGNAL_NAME)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            IDictionary<string, string> activities = new Dictionary<string, string>();
            activities[USER_TASK_ID] = USER_TASK_ID;
            activities[BOUNDARY_ID] = BOUNDARY_ID;

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, BOUNDARY_ID)
                    .Build();


            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated(BOUNDARY_ID, BOUNDARY_ID, SIGNAL_NAME);

            // and no event subscription for the new message name exists
            var eventSubscription = rule.RuntimeService.CreateEventSubscriptionQuery(c=> c.EventName== "new" + SIGNAL_NAME)
                .First();
            Assert.IsNull(eventSubscription);
            Assert.AreEqual(1, rule.RuntimeService.CreateEventSubscriptionQuery()
                .Count());

            // and it is possible to trigger the event with the old message name and successfully complete the migrated instance
            rule.ProcessEngine.RuntimeService.SignalEventReceived(SIGNAL_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateMessageBoundaryEventKeepTrigger()
        {
            // given
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                ////.Message(MESSAGE_NAME)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.Message("new" + MESSAGE_NAME)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            IDictionary<string, string> activities = new Dictionary<string, string>();
            activities[USER_TASK_ID] = USER_TASK_ID;
            activities[BOUNDARY_ID] = BOUNDARY_ID;

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, BOUNDARY_ID)
                    .Build();


            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated(BOUNDARY_ID, BOUNDARY_ID, MESSAGE_NAME);

            // and no event subscription for the new message name exists
            var eventSubscription = rule.RuntimeService.CreateEventSubscriptionQuery(c=> c.EventName== "new" + MESSAGE_NAME)
                .First();
            Assert.IsNull(eventSubscription);
            Assert.AreEqual(1, rule.RuntimeService.CreateEventSubscriptionQuery()
                .Count());

            // and it is possible to trigger the event with the old message name and successfully complete the migrated instance
            rule.ProcessEngine.RuntimeService.CorrelateMessage(MESSAGE_NAME);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }


        [Test]
        public virtual void testMigrateTimerBoundaryEventKeepTrigger()
        {
            // given
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.TimerWithDuration("PT5S")
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.TimerWithDuration("PT10M")
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            IDictionary<string, string> activities = new Dictionary<string, string>();
            activities[USER_TASK_ID] = USER_TASK_ID;
            activities[BOUNDARY_ID] = BOUNDARY_ID;

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .MapActivities(BOUNDARY_ID, BOUNDARY_ID)
                    .Build();


            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertJobMigrated(BOUNDARY_ID, BOUNDARY_ID, TimerExecuteNestedActivityJobHandler.TYPE);

            // and it is possible to trigger the event and successfully complete the migrated instance
            var managementService = rule.ManagementService;
            var job = managementService.CreateJobQuery()
                .First();

            managementService.ExecuteJob(job.Id);
            testHelper.CompleteTask(AFTER_BOUNDARY_TASK);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        //[ExpectedException(typeof(MigrationPlanValidationException))]
        public virtual void testMigrateConditionalBoundaryEventKeepTrigger()
        {
            // given
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(USER_TASK_ID)
                //.BoundaryEvent(BOUNDARY_ID)
                //.Condition(FALSE_CONDITION)
                //.UserTask(AFTER_BOUNDARY_TASK)
                //.EndEvent()
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);

            // expected migration validation exception
            //exceptionRule.Expect(typeof(MigrationPlanValidationException));
            //exceptionRule.ExpectMessage(MIGRATION_CONDITIONAL_VALIDATION_ERROR_MSG);

            // when conditional boundary event is migrated without update event trigger
            rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                .MapActivities(USER_TASK_ID, USER_TASK_ID)
                .MapActivities(BOUNDARY_ID, BOUNDARY_ID)
                .Build();
        }
    }
}