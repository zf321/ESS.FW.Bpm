using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationReceiveTaskTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationReceiveTaskTest()
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
        public virtual void testCannotMigrateActivityInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("receiveTask", "receiveTask")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertEventSubscriptionMigrated("receiveTask", "receiveTask", MessageReceiveModels.MESSAGE_NAME);

            // and it is possible to trigger the receive task
            rule.RuntimeService.CorrelateMessage(MessageReceiveModels.MESSAGE_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubscriptionProperties()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("receiveTask", "receiveTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            var eventSubscriptionBefore = testHelper.SnapshotBeforeMigration.EventSubscriptions[0];

            var eventSubscriptionsAfter = testHelper.SnapshotAfterMigration.EventSubscriptions;
            Assert.AreEqual(1, eventSubscriptionsAfter.Count);
            var eventSubscriptionAfter = eventSubscriptionsAfter[0];
            Assert.AreEqual(eventSubscriptionBefore.Created, eventSubscriptionAfter.Created);
            Assert.AreEqual(eventSubscriptionBefore.ExecutionId, eventSubscriptionAfter.ExecutionId);
            Assert.AreEqual(eventSubscriptionBefore.ProcessInstanceId, eventSubscriptionAfter.ProcessInstanceId);
        }

        [Test]
        public virtual void testMigrateEventSubscriptionChangeActivityId()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS)
                    .ChangeElementId("receiveTask", "newReceiveTask"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("receiveTask", "newReceiveTask")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertEventSubscriptionMigrated("receiveTask", "newReceiveTask",
                MessageReceiveModels.MESSAGE_NAME);

            // and it is possible to trigger the receive task
            rule.RuntimeService.CorrelateMessage(MessageReceiveModels.MESSAGE_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubscriptionPreserveMessageName()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS)
                    .RenameMessage(MessageReceiveModels.MESSAGE_NAME, "new" + MessageReceiveModels.MESSAGE_NAME));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("receiveTask", "receiveTask")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then the message event subscription's event name has not changed
            testHelper.AssertEventSubscriptionMigrated("receiveTask", "receiveTask", MessageReceiveModels.MESSAGE_NAME);

            // and it is possible to trigger the receive task
            rule.RuntimeService.CorrelateMessage(MessageReceiveModels.MESSAGE_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubscriptionUpdateMessageName()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS)
                    .RenameMessage(MessageReceiveModels.MESSAGE_NAME, "new" + MessageReceiveModels.MESSAGE_NAME));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("receiveTask", "receiveTask")
                    .UpdateEventTrigger()
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then the message event subscription's event name has not changed
            testHelper.AssertEventSubscriptionMigrated("receiveTask", MessageReceiveModels.MESSAGE_NAME, "receiveTask",
                "new" + MessageReceiveModels.MESSAGE_NAME);

            // and it is possible to trigger the event
            rule.RuntimeService.CorrelateMessage("new" + MessageReceiveModels.MESSAGE_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateParallelMultiInstanceEventSubscription()
        {
            IBpmnModelInstance parallelMiReceiveTaskProcess =
                    ModifiableBpmnModelInstance.Modify(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS)
                //.ActivityBuilder("receiveTask")
                //.MultiInstance()
                //.Parallel()
                //.Cardinality("3")
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(parallelMiReceiveTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(parallelMiReceiveTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("receiveTask#multiInstanceBody", "receiveTask#multiInstanceBody")
                    .MapActivities("receiveTask", "receiveTask")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertEventSubscriptionsMigrated("receiveTask", "receiveTask", MessageReceiveModels.MESSAGE_NAME);

            // and it is possible to trigger the receive tasks
            rule.RuntimeService.CreateMessageCorrelation(MessageReceiveModels.MESSAGE_NAME)
                .CorrelateAll();

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateSequentialMultiInstanceEventSubscription()
        {
            IBpmnModelInstance parallelMiReceiveTaskProcess =
                    ModifiableBpmnModelInstance.Modify(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS)
                //.ActivityBuilder("receiveTask")
                //.MultiInstance()
                //.Sequential()
                //.Cardinality("3")
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(parallelMiReceiveTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(parallelMiReceiveTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("receiveTask#multiInstanceBody", "receiveTask#multiInstanceBody")
                    .MapActivities("receiveTask", "receiveTask")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertEventSubscriptionsMigrated("receiveTask", "receiveTask", MessageReceiveModels.MESSAGE_NAME);

            // and it is possible to trigger the receive tasks
            for (var i = 0; i < 3; i++)
                rule.RuntimeService.CorrelateMessage(MessageReceiveModels.MESSAGE_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubscriptionAddParentScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.SUBPROCESS_RECEIVE_TASK_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("receiveTask", "receiveTask")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertEventSubscriptionMigrated("receiveTask", "receiveTask", MessageReceiveModels.MESSAGE_NAME);

            // and it is possible to trigger the receive task
            rule.RuntimeService.CorrelateMessage(MessageReceiveModels.MESSAGE_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }
    }
}