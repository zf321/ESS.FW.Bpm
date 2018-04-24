using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration.History
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationHistoricProcessInstanceTest
    {
        [SetUp]
        public virtual void initTest()
        {
            runtimeService = rule.RuntimeService;
            historyService = rule.HistoryService;


            sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var modifiedModel = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .ChangeElementId("Process", "Process2")
                .ChangeElementId("userTask", "userTask2");
            targetProcessDefinition = testHelper.DeployAndGetDefinition(modifiedModel);
            migrationPlan = runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                .MapActivities("userTask", "userTask2")
                .Build();
            runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
        }

        protected internal IHistoryService historyService;
        private readonly bool InstanceFieldsInitialized;
        protected internal IMigrationPlan migrationPlan;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;

        //============================================================================
        //===================================Migration================================
        //============================================================================
        protected internal IProcessDefinition sourceProcessDefinition;
        protected internal IProcessDefinition targetProcessDefinition;
        protected internal MigrationTestRule testHelper;

        public MigrationHistoricProcessInstanceTest()
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

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
        public virtual void testMigrateHistoryProcessInstance()
        {
            //given
            var sourceHistoryProcessInstanceQuery = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionId==sourceProcessDefinition.Id);
            var targetHistoryProcessInstanceQuery = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionId==targetProcessDefinition.Id);


            //when
            Assert.AreEqual(1, sourceHistoryProcessInstanceQuery.Count());
            Assert.AreEqual(0, targetHistoryProcessInstanceQuery.Count());
            var sourceProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == sourceProcessDefinition.Id);
            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceQuery(sourceProcessInstanceQuery)
                .Execute();

            //then
            Assert.AreEqual(0, sourceHistoryProcessInstanceQuery.Count());
            Assert.AreEqual(1, targetHistoryProcessInstanceQuery.Count());

            var instance = targetHistoryProcessInstanceQuery.First();
            Assert.AreEqual(instance.ProcessDefinitionKey, targetProcessDefinition.Key);
        }
    }
}