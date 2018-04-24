using System.Collections.Generic;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationMessageCatchEventTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationMessageCatchEventTest()
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
        public virtual void testMigrateEventSubscription()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("messageCatch", "messageCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertEventSubscriptionMigrated("messageCatch", "messageCatch", MessageReceiveModels.MESSAGE_NAME);

            // and it is possible to trigger the receive task
            rule.RuntimeService.CorrelateMessage(MessageReceiveModels.MESSAGE_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubscriptionChangeActivityId()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS)
                    .ChangeElementId("messageCatch", "newMessageCatch"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("messageCatch", "newMessageCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertEventSubscriptionMigrated("messageCatch", "newMessageCatch",
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
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.NewModel()
                .StartEvent()
                .IntermediateCatchEvent("messageCatch")
                .Message("new" + MessageReceiveModels.MESSAGE_NAME)
                .UserTask("userTask")
                .EndEvent()
                .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("messageCatch", "messageCatch")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then the message event subscription's event name has not changed
            testHelper.AssertEventSubscriptionMigrated("messageCatch", "messageCatch", MessageReceiveModels.MESSAGE_NAME);

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
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS)
                    .RenameMessage(MessageReceiveModels.MESSAGE_NAME, "new" + MessageReceiveModels.MESSAGE_NAME));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("messageCatch", "messageCatch")
                    .UpdateEventTrigger()
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then the message event subscription's event name has changed
            testHelper.AssertEventSubscriptionMigrated("messageCatch", MessageReceiveModels.MESSAGE_NAME, "messageCatch",
                "new" + MessageReceiveModels.MESSAGE_NAME);

            // and it is possible to trigger the event
            rule.RuntimeService.CorrelateMessage("new" + MessageReceiveModels.MESSAGE_NAME);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testMigrateEventSubscriptionUpdateMessageNameWithExpression()
        {
            // given
            var newMessageName = "new" + MessageReceiveModels.MESSAGE_NAME + "-${var}";
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS)
                    .RenameMessage(MessageReceiveModels.MESSAGE_NAME, newMessageName));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("messageCatch", "messageCatch")
                    .UpdateEventTrigger()
                    .Build();

            var variables = new Dictionary<string, object>();
            variables["var"] = "foo";

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan, variables);

            // then the message event subscription's event name has changed
            var resolvedMessageName = "new" + MessageReceiveModels.MESSAGE_NAME + "-foo";
            testHelper.AssertEventSubscriptionMigrated("messageCatch", MessageReceiveModels.MESSAGE_NAME, "messageCatch",
                resolvedMessageName);

            // and it is possible to trigger the event
            rule.RuntimeService.CorrelateMessage(resolvedMessageName);

            testHelper.CompleteTask("userTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }
    }
}