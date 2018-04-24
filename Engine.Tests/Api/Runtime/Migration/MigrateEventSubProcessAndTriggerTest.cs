using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Api.Runtime.Migration.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class MigrateEventSubProcessAndTriggerTest
    [TestFixture]
    public class MigrateEventSubProcessAndTriggerTest
    {
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        [SetUp]
        public virtual void setUp()
        {
            ClockUtil.CurrentTime = DateTime.Now; // lock time so that timer job is effectively not updated
        }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter public api.Runtime.Migration.util.BpmnEventFactory eventFactory;
        public BpmnEventFactory eventFactory;
        private readonly bool InstanceFieldsInitialized;

        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrateEventSubProcessAndTriggerTest()
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

        [Test]
        public virtual void testMigrateEventSubprocessSignalTrigger()
        {
            var processModel = ProcessModels.OneTaskProcess.Clone();
            var eventTrigger = eventFactory.AddEventSubProcess(rule.ProcessEngine, processModel,
                ProcessModels.ProcessKey, "eventSubProcess", "eventSubProcessStart");
            ModifiableBpmnModelInstance.Wrap(processModel)
                .StartEventBuilder("eventSubProcessStart")
                .UserTask("eventSubProcessTask")
                .EndEvent()
                .Done();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(processModel);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(processModel);

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .MapActivities("eventSubProcessStart", "eventSubProcessStart")
                    .UpdateEventTrigger()
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            eventTrigger.AssertEventTriggerMigrated(testHelper, "eventSubProcessStart");

            // and it is possible to trigger the event subprocess
            eventTrigger.Trigger(processInstance.Id);
            Assert.AreEqual(1, rule.TaskService.CreateTaskQuery()
                .Count());

            // and complete the process instance
            testHelper.CompleteTask("eventSubProcessTask");
            testHelper.AssertProcessEnded(processInstance.Id);
        }
    }
}