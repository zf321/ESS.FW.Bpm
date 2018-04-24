using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationIntermediateConditionalEventTest
    {
        protected internal const string VAR_NAME = "variable";
        protected internal const string NEW_CONDITION_ID = "newCondition";
        protected internal const string NEW_VAR_CONDITION = "${variable == 2}";


        public static readonly IBpmnModelInstance ONE_CONDITION_PROCESS = ProcessModels.NewModel()
            .StartEvent()
            .IntermediateCatchEvent(ConditionalModels.CONDITION_ID)
            .ConditionalEventDefinition("test")
            //.Condition(ConditionalModels.VAR_CONDITION)
            //.ConditionalEventDefinitionDone()
            //.UserTask(ConditionalModels.USER_TASK_ID)
            //.EndEvent()
            .Done();

        private readonly bool InstanceFieldsInitialized;
        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationIntermediateConditionalEventTest()
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
        ////public ExpectedException exceptionRule = ExpectedException.None();

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        [Test]
        public virtual void testMigrateEventSubscription()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ONE_CONDITION_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ONE_CONDITION_PROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(ConditionalModels.CONDITION_ID, ConditionalModels.CONDITION_ID)
                    .UpdateEventTrigger()
                    .Build();

            //when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertEventSubscriptionMigrated(ConditionalModels.CONDITION_ID, ConditionalModels.CONDITION_ID,
                null);

            //then it is possible to trigger the conditional event
            testHelper.SetVariable(processInstance.Id, VAR_NAME, "1");

            testHelper.CompleteTask(ConditionalModels.USER_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        //[ExpectedException(typeof(MigrationPlanValidationException))]
        public virtual void testMigrateConditionalEventWithoutUpdateTrigger()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ONE_CONDITION_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ONE_CONDITION_PROCESS);

            //expect migration validation exception
            //exceptionRule.Expect(typeof(MigrationPlanValidationException));
            //exceptionRule.ExpectMessage(MIGRATION_CONDITIONAL_VALIDATION_ERROR_MSG);

            //when conditional event is migrated without update event trigger
            rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                .MapActivities(ConditionalModels.CONDITION_ID, ConditionalModels.CONDITION_ID)
                .Build();
        }

        [Test]
        public virtual void testMigrateEventSubscriptionChangeCondition()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ONE_CONDITION_PROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess()
                .StartEvent()
                .IntermediateCatchEvent(NEW_CONDITION_ID)
                .ConditionalEventDefinition()
                //.Condition(NEW_VAR_CONDITION)
                //.ConditionalEventDefinitionDone()
                //.UserTask(ConditionalModels.USER_TASK_ID)
                .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(ConditionalModels.CONDITION_ID, NEW_CONDITION_ID)
                    .UpdateEventTrigger()
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertEventSubscriptionMigrated(ConditionalModels.CONDITION_ID, NEW_CONDITION_ID, null);

            //and var is set with value of old condition
            testHelper.SetVariable(processInstance.Id, VAR_NAME, "1");

            //then nothing happens
            Assert.IsNull(rule.TaskService.CreateTaskQuery()
                .First());

            //when correct value is set
            testHelper.SetVariable(processInstance.Id, VAR_NAME, "2");

            //then condition is satisfied
            testHelper.CompleteTask(ConditionalModels.USER_TASK_ID);
            testHelper.AssertProcessEnded(processInstance.Id);
        }
    }
}