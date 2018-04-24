using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration.History
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationHistoricActivityInstanceTest
    {
        [SetUp]
        public virtual void initServices()
        {
            historyService = rule.HistoryService;
            runtimeService = rule.RuntimeService;
        }

        protected internal IHistoryService historyService;
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal MigrationTestRule testHelper;

        public MigrationHistoricActivityInstanceTest()
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
        public virtual void testMigrateHistoricSubProcessInstance()
        {
            //given
            var processDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskSubprocessProcess);

            var migrationPlan = rule.RuntimeService.CreateMigrationPlan(processDefinition.Id, processDefinition.Id)
                .MapEqualActivities()
                .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(processDefinition.Id);

            // when
            rule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var historicInstances = historyService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId== processInstance.Id)
                /*.Unfinished()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/
                
                .ToList();

            Assert.AreEqual(2, historicInstances.Count);

            AssertMigratedTo(historicInstances[0], processDefinition, "subProcess");
            AssertMigratedTo(historicInstances[1], processDefinition, "userTask");
            Assert.AreEqual(processInstance.Id, historicInstances[0].ParentActivityInstanceId);
            Assert.AreEqual(historicInstances[0].Id, historicInstances[1].ParentActivityInstanceId);
        }

        protected internal virtual void AssertMigratedTo(IHistoricActivityInstance activityInstance,
            IProcessDefinition processDefinition, string activityId)
        {
            Assert.AreEqual(processDefinition.Id, activityInstance.ProcessDefinitionId);
            Assert.AreEqual(processDefinition.Key, activityInstance.ProcessDefinitionKey);
            Assert.AreEqual(activityId, activityInstance.ActivityId);
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
        public virtual void testHistoricActivityInstanceBecomeScope()
        {
            //given
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);

            var migrationPlan = rule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                .MapEqualActivities()
                .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);

            // when
            rule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var historicInstances = historyService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId== processInstance.Id )
                /*.Unfinished()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/
                
                .ToList();

            Assert.AreEqual(1, historicInstances.Count);

            AssertMigratedTo(historicInstances[0], targetDefinition, "userTask");
            Assert.AreEqual(processInstance.Id, historicInstances[0].ParentActivityInstanceId);
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
        public virtual void testMigrateHistoricActivityInstanceAddScope()
        {
            //given
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan = rule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                .MapActivities("userTask", "userTask")
                .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);

            // when
            rule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var historicInstances = historyService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId== processInstance.Id)
                /*.Unfinished()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/
                
                .ToList();

            Assert.AreEqual(2, historicInstances.Count);

            AssertMigratedTo(historicInstances[0], targetDefinition, "subProcess");
            AssertMigratedTo(historicInstances[1], targetDefinition, "userTask");
            Assert.AreEqual(processInstance.Id, historicInstances[0].ParentActivityInstanceId);
            Assert.AreEqual(historicInstances[0].Id, historicInstances[1].ParentActivityInstanceId);
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
        public virtual void testMigrateHistoricSubProcessRename()
        {
            //given
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    .ChangeElementId("subProcess", "newSubProcess"));

            var migrationPlan = rule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                .MapActivities("subProcess", "newSubProcess")
                .MapActivities("userTask", "userTask")
                .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);

            // when
            rule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var historicInstances = historyService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId== processInstance.Id)
                /*.Unfinished()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/
                
                .ToList();

            Assert.AreEqual(2, historicInstances.Count);

            AssertMigratedTo(historicInstances[0], targetDefinition, "newSubProcess");
            AssertMigratedTo(historicInstances[1], targetDefinition, "userTask");
            Assert.AreEqual(processInstance.Id, historicInstances[0].ParentActivityInstanceId);
            Assert.AreEqual(historicInstances[0].Id, historicInstances[1].ParentActivityInstanceId);
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
        public virtual void testMigrateHistoryActivityInstance()
        {
            //given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .ChangeElementId("Process", "Process2")
                    .ChangeElementId("userTask", "userTask2")
                    .ChangeElementName("userTask", "new activity name"));

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask2")
                    .Build();

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var sourceHistoryActivityInstanceQuery = historyService.CreateHistoricActivityInstanceQuery(c=>c.ProcessDefinitionId==sourceProcessDefinition.Id);
            var targetHistoryActivityInstanceQuery = historyService.CreateHistoricActivityInstanceQuery(c=>c.ProcessDefinitionId==targetProcessDefinition.Id);

            //when
            Assert.AreEqual(2, sourceHistoryActivityInstanceQuery.Count());
            Assert.AreEqual(0, targetHistoryActivityInstanceQuery.Count());
            var sourceProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == sourceProcessDefinition.Id);
            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceQuery(sourceProcessInstanceQuery)
                .Execute();

            // then one instance of the start event still belongs to the source process
            // and one active user task instances is now migrated to the target process
            Assert.AreEqual(1, sourceHistoryActivityInstanceQuery.Count());
            Assert.AreEqual(1, targetHistoryActivityInstanceQuery.Count());

            var instance = targetHistoryActivityInstanceQuery.First();
            AssertMigratedTo(instance, targetProcessDefinition, "userTask2");
            Assert.AreEqual("new activity name", instance.ActivityName);
            Assert.AreEqual(processInstance.Id, instance.ParentActivityInstanceId);
            Assert.AreEqual("userTask", instance.ActivityType);
        }
    }
}