using System.Collections.Generic;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    [TestFixture]
    public class MigrationNestedEventSubProcessTest
    {
        protected internal const string USER_TASK_ID = "userTask";
        protected internal const string EVENT_SUB_PROCESS_START_ID = "eventSubProcessStart";
        protected internal const string EVENT_SUB_PROCESS_TASK_ID = "eventSubProcessTask";
        public const string TIMER_DATE = "2016-02-11T12:13:14Z";

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameter public MigrationEventSubProcessTestConfiguration configuration;
        public MigrationEventSubProcessTestConfiguration configuration;

        protected static ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected MigrationTestRule testHelper = new MigrationTestRule(rule);


        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameterized.Parameters(name = "{index}: {0}") public static java.util.Collection<Object[]> data()
        public static ICollection<object[]> data()
        {
            //JAVA TO C# CONVERTER TODO Resources.Task: The following anonymous inner class could not be converted:
            //		return java.util.(new Object[][]{ { new MigrationEventSubProcessTestConfiguration()
            //	{ //message event sub process configuration
            //		  @@Override public IBpmnModelInstance getSourceProcess()
            //		  {
            //			return EventSubProcessModels.NESTED_EVENT_SUB_PROCESS_PROCESS;
            //		  }
            //
            //		  @@Override public String getEventName()
            //		  {
            //			return EventSubProcessModels.MESSAGE_NAME;
            //		  }
            //
            //		  @@Override public void triggerEventSubProcess(MigrationTestRule testHelper)
            //		  {
            //			TestHelper.CorrelateMessage(EventSubProcessModels.MESSAGE_NAME);
            //		  }
            //
            //		  @@Override public String toString()
            //		  {
            //			return "MigrateMessageEventSubProcess";
            //		  }
            //		}
            //  }
            // ,
            // {
            //	  //signal event sub process configuration
            //		  new MigrationEventSubProcessTestConfigurationAnonymousInnerClass2(this)
            // }
            //	 ,
            //	 {
            //	  //timer event sub process configuration
            //		  new MigrationEventSubProcessTestConfigurationAnonymousInnerClass3(this)
            //	 }
            //	 ,
            //	 {
            //	  //conditional event sub process configuration
            //		  new MigrationEventSubProcessTestConfigurationAnonymousInnerClass4(this)
            //	 }
            //}
            //   );
            return null;
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain //ruleChain = RuleChain.outerRule(rule).around(testHelper);

        [Test]
        public void testMapUserTaskSiblingOfEventSubProcess()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(configuration.SourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(configuration.SourceProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(USER_TASK_ID)
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope(EventSubProcessModels.SUB_PROCESS_ID)
                    .Activity(USER_TASK_ID, testHelper.GetSingleActivityInstanceBeforeMigration(USER_TASK_ID)
                        .Id)
                    .Done());

            configuration.AssertMigration(testHelper);

            // and it is possible to successfully complete the migrated instance
            testHelper.CompleteTask(USER_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public void testMapUserTaskSiblingOfEventSubProcessAndTriggerEvent()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(configuration.SourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(configuration.SourceProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(USER_TASK_ID, USER_TASK_ID)
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to trigger event sub process and successfully complete the migrated instance
            configuration.triggerEventSubProcess(testHelper);
            testHelper.CompleteTask(EVENT_SUB_PROCESS_TASK_ID);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        public abstract class MigrationEventSubProcessTestConfiguration
        {
            public abstract IBpmnModelInstance SourceProcess { get; }

            public abstract string EventName { get; }

            public virtual void AssertMigration(MigrationTestRule testHelper)
            {
                testHelper.AssertEventSubscriptionRemoved(EVENT_SUB_PROCESS_START_ID, EventName);
                testHelper.AssertEventSubscriptionCreated(EVENT_SUB_PROCESS_START_ID, EventName);
            }

            public abstract void triggerEventSubProcess(MigrationTestRule testHelper);
        }
    }
}