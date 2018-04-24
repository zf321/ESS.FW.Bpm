using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationEventBasedGatewayTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationEventBasedGatewayTest()
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
        public virtual void testMigrateGatewayExecutionTree()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("eventBasedGateway")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("eventBasedGateway"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("eventBasedGateway",
                        testHelper.GetSingleActivityInstanceBeforeMigration("eventBasedGateway")
                            .Id)
                    .Done());
        }

        [Test]
        public virtual void testMigrateGatewayWithTimerEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertIntermediateTimerJobRemoved("timerCatch");
            testHelper.AssertIntermediateTimerJobCreated("timerCatch");

            var timerJob = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(timerJob.Id);

            testHelper.CompleteTask("afterTimerCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayWithMessageEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("messageCatch", EventBasedGatewayModels.MESSAGE_NAME);
            testHelper.AssertEventSubscriptionCreated("messageCatch", EventBasedGatewayModels.MESSAGE_NAME);

            rule.RuntimeService.CorrelateMessage(EventBasedGatewayModels.MESSAGE_NAME);

            testHelper.CompleteTask("afterMessageCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayWithSignalEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("signalCatch", EventBasedGatewayModels.SIGNAL_NAME);
            testHelper.AssertEventSubscriptionCreated("signalCatch", EventBasedGatewayModels.SIGNAL_NAME);

            rule.RuntimeService.SignalEventReceived(EventBasedGatewayModels.SIGNAL_NAME);

            testHelper.CompleteTask("afterSignalCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayWithTimerEventMapEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("timerCatch", "timerCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertIntermediateTimerJobMigrated("timerCatch", "timerCatch");

            var timerJob = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(timerJob.Id);

            testHelper.CompleteTask("afterTimerCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayWithMessageEventMapEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("messageCatch", "messageCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated("messageCatch", "messageCatch",
                EventBasedGatewayModels.MESSAGE_NAME);

            rule.RuntimeService.CorrelateMessage(EventBasedGatewayModels.MESSAGE_NAME);

            testHelper.CompleteTask("afterMessageCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayWithSignalEventMapEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("signalCatch", "signalCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated("signalCatch", "signalCatch", EventBasedGatewayModels.SIGNAL_NAME);

            rule.RuntimeService.SignalEventReceived(EventBasedGatewayModels.SIGNAL_NAME);

            testHelper.CompleteTask("afterSignalCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayAddTimerEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS)
                    //.FlowNodeBuilder("eventBasedGateway")
                    //.IntermediateCatchEvent("newTimerCatch")
                    //.TimerWithDuration("PT50M")
                    //.UserTask("afterNewTimerCatch")
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("timerCatch", "timerCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertIntermediateTimerJobCreated("newTimerCatch");
            testHelper.AssertIntermediateTimerJobMigrated("timerCatch", "timerCatch");

            var newTimerJob = rule.ManagementService.CreateJobQuery(c=> c.ActivityId =="newTimerCatch")
                .First();
            rule.ManagementService.ExecuteJob(newTimerJob.Id);

            testHelper.CompleteTask("afterNewTimerCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayAddMessageEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS)
                    //.FlowNodeBuilder("eventBasedGateway")
                    //.IntermediateCatchEvent("newMessageCatch")
                    //.Message("new" + EventBasedGatewayModels.MESSAGE_NAME)
                    //.UserTask("afterNewMessageCatch")
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("messageCatch", "messageCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("newMessageCatch", "new" + EventBasedGatewayModels.MESSAGE_NAME);
            testHelper.AssertEventSubscriptionMigrated("messageCatch", "messageCatch",
                EventBasedGatewayModels.MESSAGE_NAME);

            rule.RuntimeService.CorrelateMessage("new" + EventBasedGatewayModels.MESSAGE_NAME);

            testHelper.CompleteTask("afterNewMessageCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayAddSignalEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS)
                    //.FlowNodeBuilder("eventBasedGateway")
                    //.IntermediateCatchEvent("newSignalCatch")
                    //.Signal("new" + EventBasedGatewayModels.SIGNAL_NAME)
                    //.UserTask("afterNewSignalCatch")
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("signalCatch", "signalCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionCreated("newSignalCatch", "new" + EventBasedGatewayModels.SIGNAL_NAME);
            testHelper.AssertEventSubscriptionMigrated("signalCatch", "signalCatch", EventBasedGatewayModels.SIGNAL_NAME);

            rule.RuntimeService.SignalEventReceived("new" + EventBasedGatewayModels.SIGNAL_NAME);

            testHelper.CompleteTask("afterNewSignalCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayRemoveTimerEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS)
                    //.FlowNodeBuilder("eventBasedGateway")
                    //.IntermediateCatchEvent("oldTimerCatch")
                    //.TimerWithDuration("PT50M")
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("timerCatch", "timerCatch")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertIntermediateTimerJobRemoved("oldTimerCatch");
            testHelper.AssertIntermediateTimerJobMigrated("timerCatch", "timerCatch");
        }

        [Test]
        public virtual void testMigrateGatewayRemoveMessageEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS)
                    //.FlowNodeBuilder("eventBasedGateway")
                    //.IntermediateCatchEvent("oldMessageCatch")
                    //.Message("old" + EventBasedGatewayModels.MESSAGE_NAME)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("messageCatch", "messageCatch")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("oldMessageCatch", "old" + EventBasedGatewayModels.MESSAGE_NAME);
            testHelper.AssertEventSubscriptionMigrated("messageCatch", "messageCatch",
                EventBasedGatewayModels.MESSAGE_NAME);
        }

        [Test]
        public virtual void testMigrateGatewayRemoveSignalEvent()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS)
                    //.FlowNodeBuilder("eventBasedGateway")
                    //.IntermediateCatchEvent("oldSignalCatch")
                    //.Signal("old" + EventBasedGatewayModels.SIGNAL_NAME)
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("signalCatch", "signalCatch")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionRemoved("oldSignalCatch", "old" + EventBasedGatewayModels.SIGNAL_NAME);
            testHelper.AssertEventSubscriptionMigrated("signalCatch", "signalCatch", EventBasedGatewayModels.SIGNAL_NAME);
        }

        [Test]
        public virtual void testMigrateGatewayWithTimerEventChangeId()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS)
                    .ChangeElementId("timerCatch", "newTimerCatch"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("timerCatch", "newTimerCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertIntermediateTimerJobMigrated("timerCatch", "newTimerCatch");

            var timerJob = testHelper.SnapshotAfterMigration.Jobs[0];
            rule.ManagementService.ExecuteJob(timerJob.Id);

            testHelper.CompleteTask("afterTimerCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayWithMessageEventChangeId()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        EventBasedGatewayModels.MESSAGE_EVENT_BASED_GW_PROCESS)
                    .ChangeElementId("messageCatch", "newMessageCatch"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("messageCatch", "newMessageCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated("messageCatch", "newMessageCatch",
                EventBasedGatewayModels.MESSAGE_NAME);

            rule.RuntimeService.CorrelateMessage(EventBasedGatewayModels.MESSAGE_NAME);

            testHelper.CompleteTask("afterMessageCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayWithSignalEventChangeId()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS)
                    .ChangeElementId("signalCatch", "newSignalCatch"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("signalCatch", "newSignalCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertEventSubscriptionMigrated("signalCatch", "newSignalCatch",
                EventBasedGatewayModels.SIGNAL_NAME);

            rule.RuntimeService.SignalEventReceived(EventBasedGatewayModels.SIGNAL_NAME);

            testHelper.CompleteTask("afterSignalCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayWithIncident()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .MapActivities("timerCatch", "timerCatch")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

            var timerJob = rule.ManagementService.CreateJobQuery()
                .First();
            // create an incident
            rule.ManagementService.SetJobRetries(timerJob.Id, 0);
            var incidentBeforeMigration = rule.RuntimeService.CreateIncidentQuery()
                .First();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then job and incident still exist
            testHelper.AssertIntermediateTimerJobMigrated("timerCatch", "timerCatch");

            var jobAfterMigration = testHelper.SnapshotAfterMigration.Jobs[0];

            var incidentAfterMigration = rule.RuntimeService.CreateIncidentQuery()
                .First();
            Assert.NotNull(incidentAfterMigration);

            Assert.AreEqual(incidentBeforeMigration.Id, incidentAfterMigration.Id);
            Assert.AreEqual(jobAfterMigration.Id, incidentAfterMigration.Configuration);

            Assert.AreEqual("timerCatch", incidentAfterMigration.ActivityId);
            Assert.AreEqual(targetProcessDefinition.Id, incidentAfterMigration.ProcessDefinitionId);

            // and it is possible to complete the process
            rule.ManagementService.ExecuteJob(jobAfterMigration.Id);

            testHelper.CompleteTask("afterTimerCatch");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateGatewayRemoveIncidentOnMigration()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventBasedGateway", "eventBasedGateway")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

            var timerJob = rule.ManagementService.CreateJobQuery()
                .First();
            // create an incident
            rule.ManagementService.SetJobRetries(timerJob.Id, 0);

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the incident is gone
            Assert.AreEqual(0, rule.RuntimeService.CreateIncidentQuery()
                .Count());
        }
    }
}